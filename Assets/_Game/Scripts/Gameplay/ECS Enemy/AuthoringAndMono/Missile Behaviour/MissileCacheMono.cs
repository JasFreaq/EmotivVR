using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class MissileCacheMono : MonoBehaviour
{
    [SerializeField] private float m_laserLifetime = 6f;
    [SerializeField] private float m_laserSpeed = 15f;
    [SerializeField] private int m_laserDamage = 10;

    [SerializeField] private float m_rocketLifetime = 10f;
    [SerializeField] private float m_rocketSpeed = 9f;
    [SerializeField] private int m_rocketDamage = 20;
    
    [SerializeField] private int m_shipDamage = 50;

    [SerializeField] private uint m_randomMissileSeed;

    public float LaserLifetime => m_laserLifetime;

    public float LaserSpeed => m_laserSpeed;
    
    public int LaserDamage => m_laserDamage;

    public float RocketLifetime => m_rocketLifetime;

    public float RocketSpeed => m_rocketSpeed;
    
    public int RocketDamage => m_rocketDamage;
    
    public int ShipDamage => m_shipDamage;

    public uint RandomMissileSeed => m_randomMissileSeed;
}

public class MissileCacheBaker : Baker<MissileCacheMono>
{
    public override void Bake(MissileCacheMono authoring)
    {
        Entity missileCacheEntity = GetEntity(TransformUsageFlags.None);

        AddComponent(missileCacheEntity, new MissileCache
        {
            mLaserLifetime = authoring.LaserLifetime,
            mLaserSpeed = authoring.LaserSpeed,
            mLaserDamage = authoring.LaserDamage,
            mRocketLifetime = authoring.RocketLifetime,
            mRocketSpeed = authoring.RocketSpeed,
            mRocketDamage = authoring.RocketDamage,
            mShipDamage = authoring.ShipDamage
        });

        AddComponent<MissileLaserElement>(missileCacheEntity);

        AddComponent<MissileRocketElement>(missileCacheEntity);
        
        AddComponent<MissileSpawnElement>(missileCacheEntity);

        AddComponent(missileCacheEntity, new MissileRandomUtility
        {
            mRand = Random.CreateFromIndex(authoring.RandomMissileSeed)
        });
    }
}