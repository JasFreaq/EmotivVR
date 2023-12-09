using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct ParticleCountdownSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ParticleUpdateData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PlayerStateData playerStateData = SystemAPI.GetSingleton<PlayerStateData>();
        if (playerStateData.mIsGamePaused)
        {
            return;
        }

        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        ParticleCountdownJob particleCountdownJob = new ParticleCountdownJob
        {
            mDeltaTime = SystemAPI.Time.DeltaTime,
            mParallelCommandBuffer = commandBuffer.AsParallelWriter()
        };

        state.Dependency = particleCountdownJob.ScheduleParallel(state.Dependency);
        state.Dependency.Complete();

        commandBuffer.Playback(state.EntityManager);
        commandBuffer.Dispose();
    }
}

[BurstCompile]
public partial struct ParticleCountdownJob : IJobEntity
{
    public float mDeltaTime;

    public EntityCommandBuffer.ParallelWriter mParallelCommandBuffer;

    [BurstCompile]
    public void Execute([ChunkIndexInQuery] int sortKey, in Entity entity, ref ParticleUpdateData particleUpdateData)
    {
        particleUpdateData.mParticleLifetimeCounter -= mDeltaTime;
        if (particleUpdateData.mParticleLifetimeCounter <= 0f)
        {
            mParallelCommandBuffer.DestroyEntity(sortKey, entity);
        }
    }
}
