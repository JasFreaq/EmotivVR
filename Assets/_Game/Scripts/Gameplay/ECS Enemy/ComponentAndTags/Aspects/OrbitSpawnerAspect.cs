using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct OrbitSpawnerAspect : IAspect
{
    public readonly Entity mEntity;
    
    private readonly RefRO<OrbitSpawnerData> m_orbitSpawnerData;
    
    private readonly RefRW<EnemySpawnerData> m_enemySpawner;

    public Entity OrbitTransformPrefab => m_orbitSpawnerData.ValueRO.mOrbitTransformPrefab;

    public Entity? SpawnedEntity
    {
        get => m_enemySpawner.ValueRO.mSpawnedEntity;
        set => m_enemySpawner.ValueRW.mSpawnedEntity = value;
    }

    public int GetRandomIndex(int length)
    {
        return m_enemySpawner.ValueRW.mRandom.NextInt(length);
    }

    public float3 GetRandomSpawnPosition()
    {
        return m_enemySpawner.ValueRW.mRandom.NextFloat3(-m_orbitSpawnerData.ValueRO.mOrbitSpawnVolume * 0.5f,
            m_orbitSpawnerData.ValueRO.mOrbitSpawnVolume * 0.5f);
    }

    public float GetRandomSemiMajorAxis()
    {
        return m_enemySpawner.ValueRW.mRandom.NextFloat(m_orbitSpawnerData.ValueRO.mSemiMajorAxisRange.x,
            m_orbitSpawnerData.ValueRO.mSemiMajorAxisRange.y);
    }
    
    public float GetRandomSemiMinorAxis()
    {
        return m_enemySpawner.ValueRW.mRandom.NextFloat(m_orbitSpawnerData.ValueRO.mSemiMinorAxisRange.x,
            m_orbitSpawnerData.ValueRO.mSemiMinorAxisRange.y);
    }

    public float3 GetRandomOrbitThicknessBounds()
    {
        return m_enemySpawner.ValueRW.mRandom.NextFloat3(m_orbitSpawnerData.ValueRO.mMinOrbitThicknessBounds,
            m_orbitSpawnerData.ValueRO.mMaxOrbitThicknessBounds);
    }

    public float3 GetRandomOrbitNormal()
    {
        return math.normalizesafe(m_enemySpawner.ValueRW.mRandom.NextFloat3());
    }

    public float GetRandomSatelliteSpeed()
    {
        return m_enemySpawner.ValueRW.mRandom.NextFloat(m_orbitSpawnerData.ValueRO.mSatelliteSpeedRange.x,
            m_orbitSpawnerData.ValueRO.mSatelliteSpeedRange.y);
    }

    public int GetRandomLasersFiredPerSecond()
    {
        return m_enemySpawner.ValueRW.mRandom.NextInt(m_orbitSpawnerData.ValueRO.mLasersFiredPerSecondRange.x,
            m_orbitSpawnerData.ValueRO.mLasersFiredPerSecondRange.y);
    }

    public float GetRandomFollowSpeed()
    {
        return m_enemySpawner.ValueRW.mRandom.NextFloat(m_orbitSpawnerData.ValueRO.mFollowSpeedRange.x,
            m_orbitSpawnerData.ValueRO.mFollowSpeedRange.y);
    }

    public int GetRandomSatelliteCount()
    {
        return m_enemySpawner.ValueRW.mRandom.NextInt(m_orbitSpawnerData.ValueRO.mSatelliteCountRange.x,
            m_orbitSpawnerData.ValueRO.mSatelliteCountRange.y);
    }

    public float3 GetRandomOrbitMemberHalfBounds()
    {
        return m_enemySpawner.ValueRW.mRandom.NextFloat3(m_orbitSpawnerData.ValueRO.mMinOrbitMemberHalfBounds,
            m_orbitSpawnerData.ValueRO.mMaxOrbitMemberHalfBounds);
    }

    public uint GetRandomSeed()
    {
        return m_enemySpawner.ValueRW.mRandom.NextUInt();
    }
}
