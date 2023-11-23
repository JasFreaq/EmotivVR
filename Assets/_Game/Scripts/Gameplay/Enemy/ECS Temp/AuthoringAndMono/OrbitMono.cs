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
    [SerializeField] private int3 m_orbitMemberHalfRange = new int3(2, 2, 2);
    [SerializeField] private float3 m_orbitThicknessRange = new float3(2, 2, 2);
    [SerializeField] private float3 m_orbitNormal = math.forward();
    [SerializeField] private uint m_orbitRandomSeed;

    [Header("Satellites")]
    [SerializeField] private GameObject m_satellitePrefab;
    [SerializeField] private float m_satelliteSpeed = 60f;
    [SerializeField] private int m_satelliteCount = 100;

    public float SemiMajorAxis => m_semiMajorAxis;

    public float SemiMinorAxis => m_semiMinorAxis;

    public int3 orbitMemberHalfRange => m_orbitMemberHalfRange;

    public float3 OrbitThicknessRange => m_orbitThicknessRange;

    public float3 OrbitNormal => m_orbitNormal;

    public uint OrbitRandomSeed => m_orbitRandomSeed;

    public GameObject SatellitePrefab => m_satellitePrefab;

    public float SatelliteSpeed => m_satelliteSpeed;

    public int SatelliteCount => m_satelliteCount;

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
            mOrbitMemberRange = authoring.orbitMemberHalfRange,
            mOrbitThicknessRange = authoring.OrbitThicknessRange,
            mOrbitNormal = authoring.OrbitNormal,
            mSatellitePrefab = GetEntity(authoring.SatellitePrefab, TransformUsageFlags.Dynamic),
            mSatelliteSpeed = authoring.SatelliteSpeed,
            mSatelliteCount = authoring.SatelliteCount
        });

        AddComponent(orbitEntity, new OrbitRandom
        {
            mRand = Random.CreateFromIndex(authoring.OrbitRandomSeed)
        });

        AddComponent(orbitEntity, new OrbitSpawnDataCache
        {
            mGenerationTimer = 0f,
            mTotalGenerationTime = 360f / authoring.SatelliteSpeed,
            mSatellitePerUnitTime = authoring.SatelliteCount / (360f / authoring.SatelliteSpeed),
            mSpawnTimeCounter = 0
        });
    }
}