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
[UpdateAfter(typeof(OrbitSpawnerSystem))]
public partial struct SatelliteSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerStateData>();
        state.RequireForUpdate<OrbitSpawnerData>();
        state.RequireForUpdate<OrbitProperties>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PlayerStateData playerStateData = SystemAPI.GetSingleton<PlayerStateData>();
        if (playerStateData.mIsGamePaused)
        {
            return;
        }

        float deltaTime = SystemAPI.Time.DeltaTime;

        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        
        foreach ((RefRO<OrbitProperties> orbitProperties, Entity orbitEntity) in 
                 SystemAPI.Query<RefRO<OrbitProperties>>().WithEntityAccess())
        {
            if (orbitProperties.ValueRO.mOrbitSpawner != default) 
            {
                RefRW<EnemySpawnerData> enemySpawner =
                    SystemAPI.GetComponentRW<EnemySpawnerData>(orbitProperties.ValueRO.mOrbitSpawner);

                if (enemySpawner.ValueRO.mSpawnedEntity == default)
                    enemySpawner.ValueRW.mSpawnedEntity = orbitEntity;
            }

            if (state.EntityManager.HasComponent<OrbitSpawnData>(orbitEntity))
            {
                OrbitSpawnAspect orbitSpawnAspect = SystemAPI.GetAspect<OrbitSpawnAspect>(orbitEntity);
                
                if (orbitSpawnAspect.GenerationTimer - float.Epsilon < orbitSpawnAspect.TotalGenerationTime)
                {
                    orbitSpawnAspect.GenerationTimer += deltaTime;
                    orbitSpawnAspect.SpawnTimeCounter += orbitSpawnAspect.SatellitePerUnitTime * deltaTime;

                    int shipsToGenerate = Mathf.FloorToInt(orbitSpawnAspect.SpawnTimeCounter);
                    orbitSpawnAspect.SpawnTimeCounter -= shipsToGenerate;

                    OrbitAspect orbitAspect = SystemAPI.GetAspect<OrbitAspect>(orbitEntity);
                    
                    for (int i = 0; i < shipsToGenerate; i++)
                    {
                        Entity satelliteEntity = commandBuffer.Instantiate(orbitSpawnAspect.SatellitePrefab);
                        
                        float3 randomOffset = orbitSpawnAspect.GetRandomOffset();
                        
                        float3 randomPosition = orbitAspect.GetRandomPosition(randomOffset);

                        quaternion rotation = orbitAspect.GetRotation(randomPosition);
                        
                        LocalTransform spawnTransform = new LocalTransform
                        {
                            Position = randomPosition,
                            Rotation = rotation,
                            Scale = 1f
                        };
                        commandBuffer.SetComponent(satelliteEntity, spawnTransform);

                        SatelliteData satelliteData = new SatelliteData
                        {
                            mTargetOrbit = orbitEntity,
                            mSpawnOffset = randomOffset,
                            mCurrentAngle = 0f
                        };
                        commandBuffer.AddComponent(satelliteEntity, satelliteData);
                    }
                }
                else
                {
                    commandBuffer.RemoveComponent<OrbitSpawnData>(orbitEntity);
                }
            }
        }

        commandBuffer.Playback(state.EntityManager);
    }
}
