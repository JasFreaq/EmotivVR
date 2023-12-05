using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class OrbitSpawnerMono : MonoBehaviour
{
    [Header("Orbit")] 
    [SerializeField] private GameObject m_orbitTransformPrefab;
    [SerializeField] private float3 m_orbitSpawnVolume;
    [SerializeField] private float2 m_semiMajorAxisRange;
    [SerializeField] private float2 m_semiMinorAxisRange;
    [SerializeField] private int3 m_minOrbitMemberHalfBounds;
    [SerializeField] private int3 m_maxOrbitMemberHalfBounds;
    [SerializeField] private float3 m_minOrbitThicknessBounds;
    [SerializeField] private float3 m_maxOrbitThicknessBounds;
    [SerializeField] private uint m_orbitSpawnerRandomSeed;

    [Header("Satellites")]
    [SerializeField] private float2 m_satelliteSpeedRange;
    [SerializeField] private int2 m_satelliteCountRange;

    [Header("Missile")]
    [SerializeField] private int2 m_lasersFiredPerSecondRange;

    [Header("Follow")]
    [SerializeField] private float2 m_followSpeedRange;

    public GameObject OrbitTransformPrefab => m_orbitTransformPrefab;

    public float3 OrbitSpawnVolume => m_orbitSpawnVolume;

    public float2 SemiMajorAxisRange => m_semiMajorAxisRange;

    public float2 SemiMinorAxisRange => m_semiMinorAxisRange;
    
    public int3 MinOrbitMemberHalfBounds => m_minOrbitMemberHalfBounds;
    
    public int3 MaxOrbitMemberHalfBounds => m_maxOrbitMemberHalfBounds;
    
    public float3 MinOrbitThicknessBounds => m_minOrbitThicknessBounds;
    
    public float3 MaxOrbitThicknessBounds => m_maxOrbitThicknessBounds;
    
    public uint OrbitSpawnerRandomSeed => m_orbitSpawnerRandomSeed;
    
    public float2 SatelliteSpeedRange => m_satelliteSpeedRange;
    
    public int2 SatelliteCountRange => m_satelliteCountRange;
    
    public int2 LasersFiredPerSecondRange => m_lasersFiredPerSecondRange;
    
    public float2 FollowSpeedRange => m_followSpeedRange;
}

public class OrbitSpawnerBaker : Baker<OrbitSpawnerMono>
{
    public override void Bake(OrbitSpawnerMono authoring)
    {
        Entity orbitSpawnerEntity = GetEntity(TransformUsageFlags.None);

        AddComponent(orbitSpawnerEntity, new OrbitSpawnerData
        {
            mOrbitTransformPrefab = GetEntity(authoring.OrbitTransformPrefab, TransformUsageFlags.Dynamic),
            mOrbitSpawnVolume = authoring.OrbitSpawnVolume,
            mSemiMajorAxisRange = authoring.SemiMajorAxisRange,
            mSemiMinorAxisRange = authoring.SemiMinorAxisRange,
            mMinOrbitMemberHalfBounds = authoring.MinOrbitMemberHalfBounds,
            mMaxOrbitMemberHalfBounds = authoring.MaxOrbitMemberHalfBounds,
            mMinOrbitThicknessBounds = authoring.MinOrbitThicknessBounds,
            mMaxOrbitThicknessBounds = authoring.MaxOrbitThicknessBounds,
            mSatelliteSpeedRange = authoring.SatelliteSpeedRange,
            mSatelliteCountRange = authoring.SatelliteCountRange,
            mLasersFiredPerSecondRange = authoring.LasersFiredPerSecondRange,
            mFollowSpeedRange = authoring.FollowSpeedRange
        });

        AddComponent(orbitSpawnerEntity, new EnemySpawnerData
        {
            mRandom = Random.CreateFromIndex(authoring.OrbitSpawnerRandomSeed)
        });
    }
}
