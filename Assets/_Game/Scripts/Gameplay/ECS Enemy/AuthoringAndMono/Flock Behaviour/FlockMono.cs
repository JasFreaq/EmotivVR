using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class FlockMono : MonoBehaviour
{
    [Header("Flock")]
    [SerializeField] [Range(1, 63)] private int m_flockSize = 50;
    [SerializeField] private float3 m_flockSpawnBounds = new float3(10f, 10f, 10f);
    [SerializeField] private float3 m_flockSpreadRange = new float3(8f, 2f, 7f);
    [SerializeField] private float m_separationRadius = 16f;
    [SerializeField] private uint m_flockSpawnRandomSeed;
    [SerializeField] private uint m_flockUpdateRandomSeed;

    [Header("Birds")]
    [SerializeField] private float m_birdSpeed = 20f;
    [SerializeField] private float m_birdAttackRange = 75f;
    [SerializeField] private GameObject m_birdPrefab;

    [Header("Missile")]
    [SerializeField] private int m_rocketsFiredPerPatrol = 3;
    [SerializeField] private float m_rocketsFiredPerSecond = 2;

    [Header("Follow")]
    [SerializeField] private float m_followRadius = 100f;
    [SerializeField] private float m_birdsProximityForUpdate = 25f;
    [SerializeField] private float m_newDestinationInvalidityRadius = 60f;

    public int FlockSize => m_flockSize;
    
    public float3 FlockSpawnBounds => m_flockSpawnBounds;
    
    public float3 FlockSpreadRange => m_flockSpreadRange;
    
    public float SeparationRadius => m_separationRadius;
    
    public float BirdSpeed => m_birdSpeed;
    
    public float BirdAttackRange => m_birdAttackRange;

    public GameObject BirdPrefab => m_birdPrefab;

    public int RocketsFiredPerPatrol => m_rocketsFiredPerPatrol;
    
    public float RocketsFiredPerSecond => m_rocketsFiredPerSecond;

    public float FollowRadius => m_followRadius;

    public float BirdsProximityForUpdate => m_birdsProximityForUpdate;

    public float NewDestinationInvalidityRadius => m_newDestinationInvalidityRadius;

    public uint FlockSpawnRandomSeed => m_flockSpawnRandomSeed;
    
    public uint FlockUpdateRandomSeed => m_flockUpdateRandomSeed;
}

public class FlockBaker : Baker<FlockMono>
{
    public override void Bake(FlockMono authoring)
    {
        Entity flockEntity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(flockEntity, new FlockProperties
        {
            mFlockSpawner = default,
            mFlockSize = authoring.FlockSize,
            mSeparationRadius = authoring.SeparationRadius,
            mBirdSpeed = authoring.BirdSpeed,
            mBirdAttackRange = authoring.BirdAttackRange,
            mRocketsPerPatrol = authoring.RocketsFiredPerPatrol,
            mFireRateTime = 1 / authoring.RocketsFiredPerSecond
        });

        AddComponent(flockEntity,new FlockFollower
        {
            mFollowRadius = authoring.FollowRadius,
            mBirdsProximityForUpdate = authoring.BirdsProximityForUpdate,
            mNewDestinationInvalidityRadius = authoring.NewDestinationInvalidityRadius
        });

        AddComponent(flockEntity, new FlockSpawnData
        {
            mFlockSpawnBounds = authoring.FlockSpawnBounds,
            mFlockSpreadRange = authoring.FlockSpreadRange,
            mBirdPrefab = GetEntity(authoring.BirdPrefab, TransformUsageFlags.Dynamic),
            mRand = Random.CreateFromIndex(authoring.FlockSpawnRandomSeed)
        });

        AddComponent<FlockBirdElement>(flockEntity);

        AddComponent(flockEntity, new FlockUpdateData
        {
            mClosestBirdDistance = float.MaxValue,
            mRand = Random.CreateFromIndex(authoring.FlockUpdateRandomSeed)
        });
    }
}