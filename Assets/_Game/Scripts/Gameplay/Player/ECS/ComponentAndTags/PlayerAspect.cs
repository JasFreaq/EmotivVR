using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct PlayerAspect : IAspect
{
    public readonly Entity mEntity;

    private readonly RefRW<LocalTransform> m_transform;
    
    private readonly RefRO<PlayerTransformData> m_playerTransformData;
    
    private readonly RefRO<PlayerCameraProperties> m_playerCameraProperties;

    public LocalTransform Transform => m_transform.ValueRO;

    public float3 PlayerPosition => m_transform.ValueRO.Position;
    
    public PlayerCameraProperties CameraProperties => m_playerCameraProperties.ValueRO;

    public void UpdateTransform()
    {
        m_transform.ValueRW.Position = m_playerTransformData.ValueRO.mPlayerPosition;
        m_transform.ValueRW.Rotation = m_playerTransformData.ValueRO.mPlayerRotation;
    }
}
