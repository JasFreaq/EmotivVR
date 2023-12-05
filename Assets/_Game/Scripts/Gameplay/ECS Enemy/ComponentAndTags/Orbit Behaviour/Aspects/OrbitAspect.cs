using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct OrbitAspect : IAspect
{
    public readonly Entity mEntity;

    private readonly RefRO<LocalTransform> m_transform;

    private readonly RefRO<OrbitProperties> m_orbitProperties;

    private readonly RefRO<OrbitUpdateData> m_orbitUpdateData;

    public int SatelliteCount => m_orbitUpdateData.ValueRO.mOrbitSatelliteCount;

    public float3 GetRandomPosition(float3 offset)
    {
        float xPos = m_orbitProperties.ValueRO.mSemiMajorAxis + offset.x;

        float3 randomPosition = new float3(xPos, 0, offset.z) * m_orbitProperties.ValueRO.mOrbitThicknessBounds;
        
        quaternion positionRotation = quaternion.AxisAngle(angle: math.acos(math.clamp(math.dot(math.normalize(math.forward()), math.normalize(m_orbitProperties.ValueRO.mOrbitNormal)), -1f, 1f)),
            axis: math.normalize(math.cross(math.forward(), m_orbitProperties.ValueRO.mOrbitNormal)));
        
        return m_transform.ValueRO.Position + math.mul(positionRotation.value, randomPosition);
    }

    public quaternion GetRotation(float3 position)
    {
        float3 lookDirection = math.normalize(math.cross(m_transform.ValueRO.Position - position,
            m_orbitProperties.ValueRO.mOrbitNormal));

        float3 upDirection = math.normalize(m_transform.ValueRO.Position - position);

        return quaternion.LookRotation(lookDirection, upDirection);
    }
}
