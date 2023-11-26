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
public partial struct FlockSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FlockProperties>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach ((RefRO<FlockProperties> flockProperties, Entity flockEntity) in
                 SystemAPI.Query<RefRO<FlockProperties>>().WithEntityAccess())
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

        commandBuffer.Playback(state.EntityManager);

        state.Enabled = false;
    }
}
