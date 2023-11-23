using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public readonly partial struct OrbitSpawnAspect : IAspect
{
    private readonly RefRW<OrbitSpawnDataCache> m_orbitSpawnDataCache;

    public float GenerationTimer
    {
        get => m_orbitSpawnDataCache.ValueRO.mGenerationTimer;
        set => m_orbitSpawnDataCache.ValueRW.mGenerationTimer = value;
    }

    public float TotalGenerationTime => m_orbitSpawnDataCache.ValueRO.mTotalGenerationTime;

    public float SatellitePerUnitTime => m_orbitSpawnDataCache.ValueRO.mSatellitePerUnitTime;

    public float SpawnTimeCounter
    {
        get => m_orbitSpawnDataCache.ValueRO.mSpawnTimeCounter;
        set => m_orbitSpawnDataCache.ValueRW.mSpawnTimeCounter = value;
    }
}
