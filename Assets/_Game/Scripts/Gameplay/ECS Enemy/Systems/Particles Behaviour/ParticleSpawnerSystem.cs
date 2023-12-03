using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class ParticleSpawnerSystem : SystemBase
{
    private BeginSimulationEntityCommandBufferSystem m_beginSystem;
    
    [BurstCompile]
    protected override void OnCreate()
    {
        RequireForUpdate<ParticleSpawnElement>();

        m_beginSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<BeginSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        EntityCommandBuffer commandBuffer = m_beginSystem.CreateCommandBuffer();

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
    }
}
