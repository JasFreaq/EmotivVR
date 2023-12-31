using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(FlockSpawnerSystem))]
public partial struct BirdSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerStateData>();
        state.RequireForUpdate<FlockSpawnerData>();
        state.RequireForUpdate<FlockProperties>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PlayerStateData playerStateData = SystemAPI.GetSingleton<PlayerStateData>();
        if (playerStateData.mIsGamePaused)
        {
            return;
        }

        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach ((RefRO<FlockProperties> flockProperties, Entity flockEntity) in
                 SystemAPI.Query<RefRO<FlockProperties>>().WithEntityAccess())
        {
            if (flockProperties.ValueRO.mFlockSpawner != default) 
            {
                RefRW<EnemySpawnerData> enemySpawner =
                    SystemAPI.GetComponentRW<EnemySpawnerData>(flockProperties.ValueRO.mFlockSpawner);
                
                if (enemySpawner.ValueRO.mSpawnedEntity == default)
                {
                    enemySpawner.ValueRW.mSpawnedEntity = flockEntity;
                }
            }

            if (state.EntityManager.HasComponent<FlockSpawnData>(flockEntity)) 
            {
                FlockAspect flockAspect = SystemAPI.GetAspect<FlockAspect>(flockEntity);
                FlockSpawnAspect flockSpawnAspect = SystemAPI.GetAspect<FlockSpawnAspect>(flockEntity);

                for (int i = 0, l = flockAspect.FlockSize; i < l; i++)
                {
                    Entity birdEntity = commandBuffer.Instantiate(flockSpawnAspect.BirdPrefab);

                    LocalTransform spawnTransform = new LocalTransform
                    {
                        Position = flockAspect.Transform.Position + flockSpawnAspect.GetRandomOffset(),
                        Rotation = quaternion.identity,
                        Scale = 1f
                    };
                    commandBuffer.SetComponent(birdEntity, spawnTransform);

                    BirdData birdData = new BirdData
                    {
                        mOwningFlock = flockEntity
                    };
                    commandBuffer.AddComponent(birdEntity, birdData);
                }
                
                commandBuffer.RemoveComponent<FlockSpawnData>(flockEntity);
            }
        }

        commandBuffer.Playback(state.EntityManager);
    }
}
