using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public readonly partial struct FlockSpawnerAspect : IAspect
{
    public readonly Entity mEntity;

    private readonly RefRO<FlockSpawnerData> m_flockSpawnerData;

    private readonly RefRW<EnemySpawnerData> m_enemySpawner;

    public Entity FlockTransformPrefab => m_flockSpawnerData.ValueRO.mFlockTransformPrefab;

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
        return m_enemySpawner.ValueRW.mRandom.NextFloat3(-m_flockSpawnerData.ValueRO.mFlockSpawnVolume * 0.5f,
            m_flockSpawnerData.ValueRO.mFlockSpawnVolume * 0.5f);
    }

    public int GetRandomFlockSize()
    {
        return m_enemySpawner.ValueRW.mRandom.NextInt(m_flockSpawnerData.ValueRO.mFlockSizeRange.x,
            m_flockSpawnerData.ValueRO.mFlockSizeRange.y);
    }

    public float3 GetRandomFlockSpawnBounds()
    {
        return m_enemySpawner.ValueRW.mRandom.NextFloat3(m_flockSpawnerData.ValueRO.mMinFlockSpawnBounds,
            m_flockSpawnerData.ValueRO.mMaxFlockSpawnBounds);
    }
    
    public float3 GetRandomFlockSpreadRange()
    {
        return m_enemySpawner.ValueRW.mRandom.NextFloat3(m_flockSpawnerData.ValueRO.mMinFlockSpreadRange,
            m_flockSpawnerData.ValueRO.mMaxFlockSpreadRange);
    }

    public float GetRandomSeparationRadius()
    {
        return m_enemySpawner.ValueRW.mRandom.NextFloat(m_flockSpawnerData.ValueRO.mSeparationRadiusRange.x,
            m_flockSpawnerData.ValueRO.mSeparationRadiusRange.y);
    }

    public float GetRandomBirdSpeed()
    {
        return m_enemySpawner.ValueRW.mRandom.NextFloat(m_flockSpawnerData.ValueRO.mBirdSpeedRange.x,
            m_flockSpawnerData.ValueRO.mBirdSpeedRange.y);
    }
    
    public float GetRandomBirdAttackRange()
    {
        return m_enemySpawner.ValueRW.mRandom.NextFloat(m_flockSpawnerData.ValueRO.mBirdAttackRange.x,
            m_flockSpawnerData.ValueRO.mBirdAttackRange.y);
    }

    public int GetRandomRocketsFiredPerPatrol()
    {
        return m_enemySpawner.ValueRW.mRandom.NextInt(m_flockSpawnerData.ValueRO.mRocketsFiredPerPatrolRange.x,
            m_flockSpawnerData.ValueRO.mRocketsFiredPerPatrolRange.y);
    }

    public float GetRandomRocketsFiredPerSecond()
    {
        return m_enemySpawner.ValueRW.mRandom.NextFloat(m_flockSpawnerData.ValueRO.mRocketsFiredPerSecondRange.x,
            m_flockSpawnerData.ValueRO.mRocketsFiredPerSecondRange.y);
    }
    
    public float GetRandomFollowRadius()
    {
        return m_enemySpawner.ValueRW.mRandom.NextFloat(m_flockSpawnerData.ValueRO.mFollowRadiusRange.x,
            m_flockSpawnerData.ValueRO.mFollowRadiusRange.y);
    }
    
    public float GetRandomBirdsProximityForUpdate()
    {
        return m_enemySpawner.ValueRW.mRandom.NextFloat(m_flockSpawnerData.ValueRO.mBirdsProximityForUpdateRange.x,
            m_flockSpawnerData.ValueRO.mBirdsProximityForUpdateRange.y);
    }
    
    public float GetRandomNewDestinationInvalidityRadius()
    {
        return m_enemySpawner.ValueRW.mRandom.NextFloat(m_flockSpawnerData.ValueRO.mNewDestinationInvalidityRadiusRange.x,
            m_flockSpawnerData.ValueRO.mNewDestinationInvalidityRadiusRange.y);
    }

    public uint GetRandomSeed()
    {
        return m_enemySpawner.ValueRW.mRandom.NextUInt();
    }
}
