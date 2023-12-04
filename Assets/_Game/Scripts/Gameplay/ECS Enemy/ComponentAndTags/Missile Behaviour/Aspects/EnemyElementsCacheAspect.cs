using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public readonly partial struct EnemyElementsCacheAspect : IAspect
{
    public readonly Entity mEntity;

    private readonly RefRO<EnemyElementsCache> m_enemyElementsCache;

    private readonly DynamicBuffer<MissileLaserElement> m_laserPrefabsBuffer;
    
    private readonly DynamicBuffer<MissileRocketElement> m_rocketPrefabsBuffer;

    private readonly RefRW<MissileRandomUtility> m_missileRandom;

    public float LaserLifetime => m_enemyElementsCache.ValueRO.mLaserLifetime;
    
    public float LaserSpeed => m_enemyElementsCache.ValueRO.mLaserSpeed;
    
    public int LaserDamage => m_enemyElementsCache.ValueRO.mLaserDamage;
    
    public float RocketLifetime => m_enemyElementsCache.ValueRO.mRocketLifetime;
    
    public float RocketSpeed => m_enemyElementsCache.ValueRO.mRocketSpeed;
    
    public int RocketDamage => m_enemyElementsCache.ValueRO.mRocketDamage;
    
    public int ShipDamage => m_enemyElementsCache.ValueRO.mShipDamage;

    public int SatelliteScore => m_enemyElementsCache.ValueRO.mSatelliteScore;

    public int BirdScore => m_enemyElementsCache.ValueRO.mBirdScore;

    public Entity GetRandomLaser()
    {
        int laserPrefabsLength = m_laserPrefabsBuffer.Length;
        int index = m_missileRandom.ValueRW.mRand.NextInt(laserPrefabsLength);

        return m_laserPrefabsBuffer[index].mLaserPrefab;
    }
    
    public Entity GetRandomRocket()
    {
        int rocketPrefabsLength = m_rocketPrefabsBuffer.Length;
        int index = m_missileRandom.ValueRW.mRand.NextInt(rocketPrefabsLength);

        return m_rocketPrefabsBuffer[index].mRocketPrefab;
    }
}
