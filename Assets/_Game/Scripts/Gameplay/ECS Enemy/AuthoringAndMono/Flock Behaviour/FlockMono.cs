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
    [SerializeField] private float m_separationRadius = 8f;
    [SerializeField] [Range(0f, 1f)] private float m_seekWeight = 1f;
    [SerializeField] private uint m_flockSpawnRandomSeed;
    [SerializeField] private uint m_flockUpdateRandomSeed;

    [Header("Birds")]
    [SerializeField] private float m_birdSpeed = 20f;
    [SerializeField] private GameObject m_birdPrefab;

    [Header("Follow Parameters")]
    [SerializeField] private float m_followRadius = 25f;
    [SerializeField] private float m_birdsProximityForUpdate = 10f;
    [SerializeField] private float m_newDestinationInvalidityRadius = 10f;

    public int FlockSize => m_flockSize;
    
    public float3 FlockSpawnBounds => m_flockSpawnBounds;
    
    public float3 FlockSpreadRange => m_flockSpreadRange;
    
    public float SeparationRadius => m_separationRadius;
    
    public float SeekWeight => m_seekWeight;

    public GameObject BirdPrefab => m_birdPrefab;

    public float BirdSpeed => m_birdSpeed;

    public uint FlockSpawnRandomSeed => m_flockSpawnRandomSeed;
    
    public uint FlockUpdateRandomSeed => m_flockUpdateRandomSeed;

    public float FollowRadius => m_followRadius;
    
    public float BirdsProximityForUpdate => m_birdsProximityForUpdate;
    
    public float NewDestinationInvalidityRadius => m_newDestinationInvalidityRadius;
}

public class FlockBaker : Baker<FlockMono>
{
    public override void Bake(FlockMono authoring)
    {
        Entity flockEntity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(flockEntity, new FlockProperties
        {
            mFlockSize = authoring.FlockSize,
            mSeparationRadius = authoring.SeparationRadius,
            mSeekWeight = authoring.SeekWeight,
            mFlockSpeed = authoring.BirdSpeed
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