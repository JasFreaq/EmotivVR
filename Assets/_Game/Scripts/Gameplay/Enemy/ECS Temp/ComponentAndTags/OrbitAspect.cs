using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public readonly partial struct OrbitAspect : IAspect
{
    public readonly Entity mEntity;

    private readonly RefRO<LocalTransform> m_transform;

    private readonly RefRO<OrbitProperties> m_orbitProperties;

    private readonly RefRW<OrbitRandom> m_orbitRandom;

    public LocalTransform Transform => m_transform.ValueRO;

    public float SemiMajorAxis => m_orbitProperties.ValueRO.mSemiMajorAxis;

    public float SemiMinorAxis => m_orbitProperties.ValueRO.mSemiMinorAxis;

    public int3 OrbitMemberHalfRange => m_orbitProperties.ValueRO.mOrbitMemberHalfRange;

    public float3 OrbitThicknessRange => m_orbitProperties.ValueRO.mOrbitThicknessRange;

    public float3 OrbitNormal => m_orbitProperties.ValueRO.mOrbitNormal;

    public Entity SatellitePrefab => m_orbitProperties.ValueRO.mSatellitePrefab;

    public float SatelliteSpeed => m_orbitProperties.ValueRO.mSatelliteSpeed;

    public int SatelliteCount => m_orbitProperties.ValueRO.mSatelliteCount;

    public Random OrbitRandomSeed => m_orbitRandom.ValueRO.mRand;

    public float3 GetRandomOffset()
    {
        float3 range = m_orbitProperties.ValueRO.mOrbitMemberHalfRange;
        float3 offset = m_orbitRandom.ValueRW.mRand.NextFloat3(-range, range);

        return offset;
    }

    public float3 GetRandomPosition(float3 offset)
    {
        float xPos = m_orbitProperties.ValueRO.mSemiMajorAxis + offset.x;

        float3 randomPosition = new float3(xPos, 0, offset.z) * m_orbitProperties.ValueRO.mOrbitThicknessRange;

        quaternion positionRotation = quaternion.AxisAngle(angle: math.acos(math.clamp(math.dot(math.normalize(math.forward()), math.normalize(m_orbitProperties.ValueRO.mOrbitNormal)), -1f, 1f)),
            axis: math.normalize(math.cross(math.forward(), m_orbitProperties.ValueRO.mOrbitNormal)));

        return m_transform.ValueRO.Position + math.mul(positionRotation.value, randomPosition);
    }
}