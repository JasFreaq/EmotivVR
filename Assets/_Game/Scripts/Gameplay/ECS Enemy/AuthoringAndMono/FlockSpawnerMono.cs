using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class FlockSpawnerMono : MonoBehaviour
{
    [Header("Flock")]
    [SerializeField] private GameObject m_flockTransformPrefab;
    [SerializeField] private float3 m_flockSpawnVolume;
    [SerializeField] private int2 m_flockSizeRange;
    [SerializeField] private float3 m_minFlockSpawnBounds;
    [SerializeField] private float3 m_maxFlockSpawnBounds;
    [SerializeField] private float3 m_minFlockSpreadRange;
    [SerializeField] private float3 m_maxFlockSpreadRange;
    [SerializeField] private float2 m_separationRadiusRange;
    [SerializeField] private uint m_flockSpawnerRandomSeed;

    [Header("Birds")]
    [SerializeField] private float2 m_birdSpeedRange;
    [SerializeField] private float2 m_birdAttackRange;

    [Header("Missile")] 
    [SerializeField] private int2 m_rocketsFiredPerPatrolRange;
    [SerializeField] private float2 m_rocketsFiredPerSecondRange;

    [Header("Follow")] 
    [SerializeField] private float2 m_followRadiusRange;
    [SerializeField] private float2 m_birdsProximityForUpdateRange;
    [SerializeField] private float2 m_newDestinationInvalidityRadiusRange;

    public GameObject FlockTransformPrefab => m_flockTransformPrefab;

    public float3 FlockSpawnVolume => m_flockSpawnVolume;

    public int2 FlockSizeRange => m_flockSizeRange;

    public float3 MinFlockSpawnBounds => m_minFlockSpawnBounds;

    public float3 MaxFlockSpawnBounds => m_maxFlockSpawnBounds;

    public float3 MinFlockSpreadRange => m_minFlockSpreadRange;

    public float3 MaxFlockSpreadRange => m_maxFlockSpreadRange;

    public float2 SeparationRadiusRange => m_separationRadiusRange;

    public uint FlockSpawnerRandomSeed => m_flockSpawnerRandomSeed;

    public float2 BirdSpeedRange => m_birdSpeedRange;

    public float2 BirdAttackRange => m_birdAttackRange;
    
    public int2 RocketsFiredPerPatrolRange => m_rocketsFiredPerPatrolRange;

    public float2 RocketsFiredPerSecondRange => m_rocketsFiredPerSecondRange;

    public float2 FollowRadiusRange => m_followRadiusRange;

    public float2 BirdsProximityForUpdateRange => m_birdsProximityForUpdateRange;

    public float2 NewDestinationInvalidityRadiusRange => m_newDestinationInvalidityRadiusRange;
}

public class FlockSpawnerBaker : Baker<FlockSpawnerMono>
{
    public override void Bake(FlockSpawnerMono authoring)
    {
        Entity flockSpawnerEntity = GetEntity(TransformUsageFlags.None);

        AddComponent(flockSpawnerEntity, new FlockSpawnerData
        {
            mFlockTransformPrefab = GetEntity(authoring.FlockTransformPrefab,TransformUsageFlags.Dynamic),
            mFlockSpawnVolume = authoring.FlockSpawnVolume,
            mFlockSizeRange = authoring.FlockSizeRange,
            mMinFlockSpawnBounds = authoring.MinFlockSpawnBounds,
            mMaxFlockSpawnBounds = authoring.MaxFlockSpawnBounds,
            mMinFlockSpreadRange = authoring.MinFlockSpreadRange,
            mMaxFlockSpreadRange = authoring.MaxFlockSpreadRange,
            mSeparationRadiusRange = authoring.SeparationRadiusRange,
            mBirdSpeedRange = authoring.BirdSpeedRange,
            mBirdAttackRange = authoring.BirdAttackRange,
            mRocketsFiredPerPatrolRange = authoring.RocketsFiredPerPatrolRange,
            mRocketsFiredPerSecondRange = authoring.RocketsFiredPerSecondRange,
            mFollowRadiusRange = authoring.FollowRadiusRange,
            mBirdsProximityForUpdateRange = authoring.BirdsProximityForUpdateRange,
            mNewDestinationInvalidityRadiusRange = authoring.NewDestinationInvalidityRadiusRange
        });

        AddComponent(flockSpawnerEntity, new EnemySpawnerData
        {
            mRandom = Random.CreateFromIndex(authoring.FlockSpawnerRandomSeed)
        });
    }
}
