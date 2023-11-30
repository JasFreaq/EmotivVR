using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Windows;
using CapsuleCollider = Unity.Physics.CapsuleCollider;

public readonly partial struct PlayerEyeLaserAspect : IAspect
{
    public readonly Entity mEntity;

    private readonly RefRW<LocalTransform> m_transform;

    private readonly RefRO<PlayerEyeLaserData> m_laserData;
    
    private readonly RefRW<PhysicsCollider> m_collider;

    public void UpdateTransform()
    {
        m_transform.ValueRW.Position = m_laserData.ValueRO.mLaserPosition;
        m_transform.ValueRW.Rotation = m_laserData.ValueRO.mLaserRotation;
    }

    public void UpdateCollider()
    {
        if (m_laserData.ValueRO.mIsLaserActive) 
        {
            unsafe
            {
                CapsuleCollider* capsule = (CapsuleCollider*)m_collider.ValueRW.ColliderPtr;

                float capsuleInitialHeight = m_laserData.ValueRO.mCapsuleInitialHeight;

                float capsuleUpdatedHeight = capsuleInitialHeight + m_laserData.ValueRO.mLaserRange;
                float3 capsuleUpdatedCenter = new float3 { z = m_laserData.ValueRO.mLaserRange * 0.5f };

                float3 axis = math.normalizesafe(math.mul(quaternion.Euler(float3.zero), new float3 { z = 1f }));
                float halfDistance = math.max(0f, capsuleUpdatedHeight * 0.5f - capsule->Radius);
                float3 halfAxis = axis * halfDistance;

                CapsuleGeometry capsuleGeometry = capsule->Geometry;
                capsuleGeometry.Vertex0 = capsuleUpdatedCenter + halfAxis;
                capsuleGeometry.Vertex1 = capsuleUpdatedCenter - halfAxis;
                capsule->Geometry = capsuleGeometry;
            }
        }
    }
}
