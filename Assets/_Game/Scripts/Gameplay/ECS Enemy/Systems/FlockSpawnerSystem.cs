using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.PackageManager;
using UnityEngine.Rendering;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct FlockSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FlockSpawnerData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach ((RefRO<FlockSpawnerData> flockSpawnerData, Entity flockSpawnerEntity) in
                 SystemAPI.Query<RefRO<FlockSpawnerData>>().WithEntityAccess())
        {
            FlockSpawnerAspect flockSpawnerAspect = SystemAPI.GetAspect<FlockSpawnerAspect>(flockSpawnerEntity);

            if (flockSpawnerAspect.SpawnedEntity != null)
            {
                Entity spawnedFlockEntity = (Entity)flockSpawnerAspect.SpawnedEntity;
                DynamicBuffer<FlockBirdElement> birdBuffer = SystemAPI.GetBuffer<FlockBirdElement>(spawnedFlockEntity);

                if (birdBuffer.Length == 0)
                {
                    commandBuffer.DestroyEntity(spawnedFlockEntity);
                }
                else
                {
                    continue;
                }
            }

            Entity flockEntity = commandBuffer.Instantiate(flockSpawnerAspect.FlockTransformPrefab);

            LocalTransform spawnTransform = new LocalTransform
            {
                Position = flockSpawnerAspect.GetRandomSpawnPosition(),
                Rotation = quaternion.identity,
                Scale = 1f
            };
            commandBuffer.SetComponent(flockEntity, spawnTransform);

            Entity enemyElementsCacheEntity = SystemAPI.GetSingletonEntity<EnemyElementsCache>();
            DynamicBuffer<BirdPrefabElement> birdPrefabsBuffer = SystemAPI.GetBuffer<BirdPrefabElement>(enemyElementsCacheEntity);
            int birdBufferLength = birdPrefabsBuffer.Length;
            Entity birdPrefabEntity = birdPrefabsBuffer[flockSpawnerAspect.GetRandomIndex(birdBufferLength)].mBirdPrefab;

            FlockProperties flockProperties = new FlockProperties
            {
                mFlockSpawner = flockSpawnerEntity,
                mFlockSize = flockSpawnerAspect.GetRandomFlockSize(),
                mSeparationRadius = flockSpawnerAspect.GetRandomSeparationRadius(),
                mBirdSpeed = flockSpawnerAspect.GetRandomBirdSpeed(),
                mBirdAttackRange = flockSpawnerAspect.GetRandomBirdAttackRange(),
                mRocketsPerPatrol = flockSpawnerAspect.GetRandomRocketsFiredPerPatrol(),
                mFireRateTime = 1 / flockSpawnerAspect.GetRandomRocketsFiredPerSecond()
            };
            commandBuffer.AddComponent(flockEntity, flockProperties);

            FlockFollower flockFollower = new FlockFollower
            {
                mFollowRadius = flockSpawnerAspect.GetRandomFollowRadius(),
                mBirdsProximityForUpdate = flockSpawnerAspect.GetRandomBirdsProximityForUpdate(),
                mNewDestinationInvalidityRadius = flockSpawnerAspect.GetRandomNewDestinationInvalidityRadius()
            };
            commandBuffer.AddComponent(flockEntity, flockFollower);

            FlockSpawnData flockSpawnData = new FlockSpawnData
            {
                mFlockSpawnBounds = flockSpawnerAspect.GetRandomFlockSpawnBounds(),
                mFlockSpreadRange = flockSpawnerAspect.GetRandomFlockSpreadRange(),
                mBirdPrefab = birdPrefabEntity,
                mRand = Random.CreateFromIndex(flockSpawnerAspect.GetRandomSeed())
            };
            commandBuffer.AddComponent(flockEntity, flockSpawnData);

            commandBuffer.AddBuffer<FlockBirdElement>(flockEntity);

            FlockUpdateData flockUpdateData = new FlockUpdateData
            {
                mClosestBirdDistance = float.MaxValue,
                mRand = Random.CreateFromIndex(flockSpawnerAspect.GetRandomSeed())
            };
            commandBuffer.AddComponent(flockEntity, flockUpdateData);
        }

        commandBuffer.Playback(state.EntityManager);
    }
}