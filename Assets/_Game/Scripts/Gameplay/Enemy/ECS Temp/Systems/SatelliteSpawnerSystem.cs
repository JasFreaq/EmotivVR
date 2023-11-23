using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct SatelliteSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<OrbitProperties>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        
        bool allOrbitsSpawned = true;
        
        foreach ((RefRO<OrbitProperties> orbitProperties, Entity orbitEntity) in 
                 SystemAPI.Query<RefRO<OrbitProperties>>().WithEntityAccess())
        {
            if (state.EntityManager.HasComponent<OrbitSpawnDataCache>(orbitEntity))
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
                        Entity satelliteEntity = commandBuffer.Instantiate(orbitAspect.SatellitePrefab);

                        float3 randomOffset = orbitAspect.GetRandomOffset();

                        LocalTransform spawnTransform = new LocalTransform
                        {
                            Position = orbitAspect.GetRandomPosition(randomOffset),
                            Rotation = quaternion.identity,
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
                    commandBuffer.RemoveComponent<OrbitSpawnDataCache>(orbitEntity);
                }

                allOrbitsSpawned = false;
            }
        }

        commandBuffer.Playback(state.EntityManager);

        if (allOrbitsSpawned)
            state.Enabled = false;
    }
}
