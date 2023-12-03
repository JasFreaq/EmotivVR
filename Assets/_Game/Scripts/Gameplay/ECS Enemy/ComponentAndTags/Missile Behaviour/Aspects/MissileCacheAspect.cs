using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public readonly partial struct MissileCacheAspect : IAspect
{
    public readonly Entity mEntity;

    private readonly RefRO<MissileCache> m_missileCache;

    private readonly DynamicBuffer<MissileLaserElement> m_laserPrefabsBuffer;
    
    private readonly DynamicBuffer<MissileRocketElement> m_rocketPrefabsBuffer;

    private readonly RefRW<MissileRandomUtility> m_missileRandom;

    public float LaserLifetime => m_missileCache.ValueRO.mLaserLifetime;
    
    public float LaserSpeed => m_missileCache.ValueRO.mLaserSpeed;
    
    public int LaserDamage => m_missileCache.ValueRO.mLaserDamage;
    
    public float RocketLifetime => m_missileCache.ValueRO.mRocketLifetime;
    
    public float RocketSpeed => m_missileCache.ValueRO.mRocketSpeed;
    
    public int RocketDamage => m_missileCache.ValueRO.mRocketDamage;
    
    public int ShipDamage => m_missileCache.ValueRO.mShipDamage;

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
