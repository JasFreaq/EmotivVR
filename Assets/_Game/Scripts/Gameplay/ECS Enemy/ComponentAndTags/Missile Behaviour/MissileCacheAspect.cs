using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public readonly partial struct MissileCacheAspect : IAspect
{
    public readonly Entity mEntity;

    private readonly RefRO<MissileCache> m_missileCache;

    private readonly DynamicBuffer<MissileLaserElement> m_LaserPrefabsBuffer;

    private readonly RefRW<MissileRandomUtility> m_missileRandom;

    public float LaserLifetime => m_missileCache.ValueRO.mLaserLifetime;
    
    public float LaserSpeed => m_missileCache.ValueRO.mLaserSpeed;
    
    public float RocketLifetime => m_missileCache.ValueRO.mRocketLifetime;
    
    public float RocketSpeed => m_missileCache.ValueRO.mRocketSpeed;

    public Entity GetRandomLaser()
    {
        int laserPrefabsLength = m_LaserPrefabsBuffer.Length;
        int index = m_missileRandom.ValueRW.mRand.NextInt(laserPrefabsLength);

        return m_LaserPrefabsBuffer[index].mLaserPrefab;
    }
}
