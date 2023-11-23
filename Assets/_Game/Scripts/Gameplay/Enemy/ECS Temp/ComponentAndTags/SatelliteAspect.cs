using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SatelliteAspect : IComponentData
{
    public readonly Entity mEntity;

    private readonly RefRW<SatelliteData> m_satelliteData;

    public float3 SpawnOffset => m_satelliteData.ValueRO.mSpawnOffset;

    public float CurrentAngle
    {
        get => m_satelliteData.ValueRO.mCurrentAngle;
        set { m_satelliteData.ValueRW.mCurrentAngle = value; }
    }
}
