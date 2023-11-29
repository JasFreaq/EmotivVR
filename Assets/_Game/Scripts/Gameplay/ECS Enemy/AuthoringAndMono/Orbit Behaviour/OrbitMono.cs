using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class OrbitMono : MonoBehaviour
{
    [Header("Orbit")]
    [SerializeField] private float m_semiMajorAxis = 50f;
    [SerializeField] private float m_semiMinorAxis = 30f;
    [SerializeField] private int3 m_orbitMemberHalfBounds = new int3(2, 2, 2);
    [SerializeField] private float3 m_orbitThicknessBounds = new float3(2, 2, 2);
    [SerializeField] private float3 m_orbitNormal = math.forward();
    [SerializeField] private uint m_orbitSpawnRandomSeed;
    [SerializeField] private uint m_orbitUpdateRandomSeed;

    [Header("Satellites")]
    [SerializeField] private GameObject m_satellitePrefab;
    [SerializeField] private float m_satelliteSpeed = 60f;
    [SerializeField] private int m_satelliteCount = 100;

    [Header("Missile")] 
    [SerializeField] private int m_missilesFiredPerSecond = 4;

    [Header("Follow")] 
    [SerializeField] private float m_followSpeed = 10f;

    public float SemiMajorAxis => m_semiMajorAxis;

    public float SemiMinorAxis => m_semiMinorAxis;

    public int3 OrbitMemberHalfBounds => m_orbitMemberHalfBounds;

    public float3 OrbitThicknessBounds => m_orbitThicknessBounds;

    public float3 OrbitNormal => m_orbitNormal;

    public uint OrbitSpawnRandomSeed => m_orbitSpawnRandomSeed;
    
    public uint OrbitUpdateRandomSeed => m_orbitUpdateRandomSeed;

    public GameObject SatellitePrefab => m_satellitePrefab;

    public float SatelliteSpeed => m_satelliteSpeed;

    public int SatelliteCount => m_satelliteCount;
    
    public int MissilesFiredPerSecond => m_missilesFiredPerSecond;

    public float FollowSpeed => m_followSpeed;
}

public class OrbitBaker : Baker<OrbitMono>
{
    public override void Bake(OrbitMono authoring)
    {
        Entity orbitEntity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(orbitEntity, new OrbitProperties
        {
            mSemiMajorAxis = authoring.SemiMajorAxis,
            mSemiMinorAxis = authoring.SemiMinorAxis,
            mOrbitThicknessBounds = authoring.OrbitThicknessBounds,
            mOrbitNormal = authoring.OrbitNormal,
            mSatelliteSpeed = authoring.SatelliteSpeed
        });
        
        AddComponent(orbitEntity, new OrbitFollower
        {
            mFollowSpeed = authoring.FollowSpeed
        });

        AddComponent(orbitEntity, new OrbitSpawnData
        {
            mGenerationTimer = 0f,
            mTotalGenerationTime = 360f / authoring.SatelliteSpeed,
            mSatellitePerUnitTime = authoring.SatelliteCount / (360f / authoring.SatelliteSpeed),
            mSpawnTimeCounter = 0,
            mOrbitMemberHalfBounds = authoring.OrbitMemberHalfBounds,
            mSatellitePrefab = GetEntity(authoring.SatellitePrefab, TransformUsageFlags.Dynamic),
            mRand = Random.CreateFromIndex(authoring.OrbitSpawnRandomSeed)
        });

        AddComponent(orbitEntity, new OrbitUpdateData
        {
            mFireRateTime = 1f / authoring.MissilesFiredPerSecond,
            mOrbitSatelliteCount = authoring.SatelliteCount,
            mRand = Random.CreateFromIndex(authoring.OrbitUpdateRandomSeed)
        });
    }
}