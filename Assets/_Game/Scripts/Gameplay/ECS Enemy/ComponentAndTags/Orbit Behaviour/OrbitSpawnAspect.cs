using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public readonly partial struct OrbitSpawnAspect : IAspect
{
    private readonly RefRW<OrbitSpawnData> m_orbitSpawnData;

    public float GenerationTimer
    {
        get => m_orbitSpawnData.ValueRO.mGenerationTimer;
        set => m_orbitSpawnData.ValueRW.mGenerationTimer = value;
    }

    public float TotalGenerationTime => m_orbitSpawnData.ValueRO.mTotalGenerationTime;

    public float SatellitePerUnitTime => m_orbitSpawnData.ValueRO.mSatellitePerUnitTime;

    public float SpawnTimeCounter
    {
        get => m_orbitSpawnData.ValueRO.mSpawnTimeCounter;
        set => m_orbitSpawnData.ValueRW.mSpawnTimeCounter = value;
    }

    public Entity SatellitePrefab => m_orbitSpawnData.ValueRO.mSatellitePrefab;

    public float3 GetRandomOffset()
    {
        float3 range = m_orbitSpawnData.ValueRO.mOrbitMemberHalfBounds;
        float3 offset = m_orbitSpawnData.ValueRW.mRand.NextFloat3(-range, range);

        return offset;
    }
}
