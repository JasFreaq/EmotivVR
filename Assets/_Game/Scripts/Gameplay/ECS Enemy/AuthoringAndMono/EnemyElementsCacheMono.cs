using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class EnemyElementsCacheMono : MonoBehaviour
{
    [SerializeField] private float m_laserLifetime = 6f;
    [SerializeField] private float m_laserSpeed = 15f;
    [SerializeField] private int m_laserDamage = 10;

    [SerializeField] private float m_rocketLifetime = 10f;
    [SerializeField] private float m_rocketSpeed = 9f;
    [SerializeField] private int m_rocketDamage = 20;
    
    [SerializeField] private int m_shipDamage = 50;
    
    [SerializeField] private int m_satelliteScore = 1000;
    [SerializeField] private int m_birdScore = 3000;

    [SerializeField] private uint m_randomMissileSeed;

    public float LaserLifetime => m_laserLifetime;

    public float LaserSpeed => m_laserSpeed;
    
    public int LaserDamage => m_laserDamage;

    public float RocketLifetime => m_rocketLifetime;

    public float RocketSpeed => m_rocketSpeed;
    
    public int RocketDamage => m_rocketDamage;
    
    public int ShipDamage => m_shipDamage;
    
    public int SatelliteScore => m_satelliteScore;
    
    public int BirdScore => m_birdScore;

    public uint RandomMissileSeed => m_randomMissileSeed;
}

public class MissileCacheBaker : Baker<EnemyElementsCacheMono>
{
    public override void Bake(EnemyElementsCacheMono authoring)
    {
        Entity missileCacheEntity = GetEntity(TransformUsageFlags.None);

        AddComponent(missileCacheEntity, new EnemyElementsCache
        {
            mLaserLifetime = authoring.LaserLifetime,
            mLaserSpeed = authoring.LaserSpeed,
            mLaserDamage = authoring.LaserDamage,
            mRocketLifetime = authoring.RocketLifetime,
            mRocketSpeed = authoring.RocketSpeed,
            mRocketDamage = authoring.RocketDamage,
            mShipDamage = authoring.ShipDamage,
            mSatelliteScore = authoring.SatelliteScore,
            mBirdScore = authoring.BirdScore
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