using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(ShipCachingSystem))]
public partial struct OrbitSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<OrbitSpawnerData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        
        foreach ((RefRO<OrbitSpawnerData> orbitSpawnerData, Entity orbitSpawnerEntity) in
                 SystemAPI.Query<RefRO<OrbitSpawnerData>>().WithEntityAccess())
        {
            OrbitSpawnerAspect orbitSpawnerAspect = SystemAPI.GetAspect<OrbitSpawnerAspect>(orbitSpawnerEntity);

            if (orbitSpawnerAspect.SpawnedEntity != null) 
            {
                Entity spawnedOrbitEntity = (Entity)orbitSpawnerAspect.SpawnedEntity;
                OrbitAspect spawnedOrbitAspect = SystemAPI.GetAspect<OrbitAspect>(spawnedOrbitEntity);

                if (spawnedOrbitAspect.SatelliteCount <= 0)
                {
                    commandBuffer.DestroyEntity(spawnedOrbitEntity);
                }
                else
                {
                    continue;
                }
            }

            Entity orbitEntity = commandBuffer.Instantiate(orbitSpawnerAspect.OrbitTransformPrefab);

            LocalTransform spawnTransform = new LocalTransform
            {
                Position = orbitSpawnerAspect.GetRandomSpawnPosition(),
                Rotation = quaternion.identity,
                Scale = 1f
            };
            commandBuffer.SetComponent(orbitEntity, spawnTransform);

            float satelliteSpeed = orbitSpawnerAspect.GetRandomSatelliteSpeed();
            int satelliteCount = orbitSpawnerAspect.GetRandomSatelliteCount();

            Entity enemyElementsCacheEntity = SystemAPI.GetSingletonEntity<EnemyElementsCache>();
            DynamicBuffer<SatellitePrefabElement> satellitePrefabsBuffer = SystemAPI.GetBuffer<SatellitePrefabElement>(enemyElementsCacheEntity);
            int satelliteBufferLength = satellitePrefabsBuffer.Length;
            Entity satellitePrefabEntity = satellitePrefabsBuffer[orbitSpawnerAspect.GetRandomIndex(satelliteBufferLength)].mSatellitePrefab;

            OrbitProperties orbitProperties = new OrbitProperties
            {
                mOrbitSpawner = orbitSpawnerAspect.mEntity,
                mSemiMajorAxis = orbitSpawnerAspect.GetRandomSemiMajorAxis(),
                mSemiMinorAxis = orbitSpawnerAspect.GetRandomSemiMinorAxis(),
                mOrbitThicknessBounds = orbitSpawnerAspect.GetRandomOrbitThicknessBounds(),
                mOrbitNormal = orbitSpawnerAspect.GetRandomOrbitNormal(),
                mSatelliteSpeed = satelliteSpeed,
                mFireRateTime = 1f / orbitSpawnerAspect.GetRandomLasersFiredPerSecond()
            };
            commandBuffer.AddComponent(orbitEntity, orbitProperties);

            OrbitFollower orbitFollower = new OrbitFollower
            {
                mFollowSpeed = orbitSpawnerAspect.GetRandomFollowSpeed()
            };
            commandBuffer.AddComponent(orbitEntity, orbitFollower);

            OrbitSpawnData orbitSpawnData = new OrbitSpawnData
            {
                mGenerationTimer = 0f,
                mTotalGenerationTime = 360f / satelliteSpeed,
                mSatellitePerUnitTime = satelliteCount / (360f / satelliteSpeed),
                mSpawnTimeCounter = 0,
                mOrbitMemberHalfBounds = orbitSpawnerAspect.GetRandomOrbitMemberHalfBounds(),
                mSatellitePrefab = satellitePrefabEntity,
                mRand = Random.CreateFromIndex(orbitSpawnerAspect.GetRandomSeed())
            };
            commandBuffer.AddComponent(orbitEntity, orbitSpawnData);

            OrbitUpdateData orbitUpdateData = new OrbitUpdateData
            {
                mOrbitSatelliteCount = satelliteCount,
                mRand = Random.CreateFromIndex(orbitSpawnerAspect.GetRandomSeed())
            };
            commandBuffer.AddComponent(orbitEntity, orbitUpdateData);
        }

        commandBuffer.Playback(state.EntityManager);
    }
}
