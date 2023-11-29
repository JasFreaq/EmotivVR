using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct ParticleSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ParticleSpawnElement>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        DynamicBuffer<ParticleSpawnElement> buffer = SystemAPI.GetSingletonBuffer<ParticleSpawnElement>();

        ParticlesCache particlesCache = SystemAPI.GetSingleton<ParticlesCache>();

        for (int i = 0, l = buffer.Length; i < l; i++)
        {
            buffer = SystemAPI.GetSingletonBuffer<ParticleSpawnElement>();

            ParticleSpawnElement particleSpawn = buffer[i];

            Entity particleEntity = commandBuffer.Instantiate(particlesCache.mTinyExplosionParticle);

            LocalTransform spawnTransform = new LocalTransform
            {
                Position = particleSpawn.mSpawnPosition,
                Rotation = particleSpawn.mSpawnRotation,
                Scale = 1f
            };
            commandBuffer.SetComponent(particleEntity, spawnTransform);

            ParticleUpdateData particleUpdate = new ParticleUpdateData
            {
                mParticleLifetimeCounter = particlesCache.mTinyExplosionLifetime
            };
            commandBuffer.AddComponent(particleEntity, particleUpdate);
        }

        buffer = SystemAPI.GetSingletonBuffer<ParticleSpawnElement>();
        buffer.Clear();

        commandBuffer.Playback(state.EntityManager);
    }
}
