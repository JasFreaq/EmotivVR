using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct SatelliteAspect : IAspect
{
    public readonly Entity mEntity;

    private readonly RefRW<LocalTransform> m_transform;

    private readonly RefRW<SatelliteData> m_satelliteData;

    public LocalTransform Transform
    {
        get => m_transform.ValueRO;
        set { m_transform.ValueRW = value; }
    }

    public float3 SpawnOffset => m_satelliteData.ValueRO.mSpawnOffset;

    public float CurrentAngle
    {
        get => m_satelliteData.ValueRO.mCurrentAngle;
        set { m_satelliteData.ValueRW.mCurrentAngle = value; }
    }

    public Entity TargetOrbit => m_satelliteData.ValueRO.mTargetOrbit;
}
