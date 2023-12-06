using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(FlockFlightSystem))]
public partial struct RocketLocomotionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<RocketData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PlayerStateData playerStateData = SystemAPI.GetSingleton<PlayerStateData>();
        if (playerStateData.mIsGamePaused)
        {
            return;
        }

        Entity missileCacheEntity = SystemAPI.GetSingletonEntity<EnemyElementsCache>();
        EnemyElementsCacheAspect enemyElementsCacheAspect = SystemAPI.GetAspect<EnemyElementsCacheAspect>(missileCacheEntity);

        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerCameraTransform>();
        PlayerAspect playerAspect = SystemAPI.GetAspect<PlayerAspect>(playerEntity);
        
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        RocketLocomotionJob rocketLocomotionJob = new RocketLocomotionJob
        {
            mDeltaTime = SystemAPI.Time.DeltaTime,
            mSpeed = enemyElementsCacheAspect.RocketSpeed,
            mPlayerPosition = playerAspect.Transform.Position,
            mParticlesCacheEntity = SystemAPI.GetSingletonEntity<ParticlesCache>(),
            mParticleSpawnBuffer = SystemAPI.GetBufferLookup<ParticleSpawnElement>(),
            mParallelCommandBuffer = commandBuffer.AsParallelWriter()
        };

        state.Dependency = rocketLocomotionJob.ScheduleParallel(state.Dependency);
        state.Dependency.Complete();

        commandBuffer.Playback(state.EntityManager);
        commandBuffer.Dispose();
    }
}

[BurstCompile]
public partial struct RocketLocomotionJob : IJobEntity
{
    public float mDeltaTime;

    public float mSpeed;
    
    public float3 mPlayerPosition;
    
    public Entity mParticlesCacheEntity;

    [NativeDisableParallelForRestriction]
    public BufferLookup<ParticleSpawnElement> mParticleSpawnBuffer;

    public EntityCommandBuffer.ParallelWriter mParallelCommandBuffer;
    
    [BurstCompile]
    public void Execute(ref LocalTransform transform, [ChunkIndexInQuery] int sortKey, in Entity entity,
        ref RocketData rocketData)
    {
        if (rocketData.mMarkedToDestroy)
        {
            SpawnExplosion(transform);
            mParallelCommandBuffer.DestroyEntity(sortKey, entity);
            return;
        }

        rocketData.mLifetimeCounter -= mDeltaTime;
        if (rocketData.mLifetimeCounter > 0f)
        {
            float3 direction = math.normalizesafe(mPlayerPosition - transform.Position);

            float3 updatedPosition = transform.Position + direction * mSpeed * mDeltaTime;

            quaternion updatedRotation = quaternion.LookRotationSafe(direction, transform.Up());

            transform.Position = updatedPosition;
            transform.Rotation = updatedRotation;
        }
        else
        {
            SpawnExplosion(transform);
            mParallelCommandBuffer.DestroyEntity(sortKey, entity);
        }
    }

    [BurstCompile]
    private void SpawnExplosion(LocalTransform transform)
    {
        if (mParticleSpawnBuffer.HasBuffer(mParticlesCacheEntity))
        {
            mParticleSpawnBuffer[mParticlesCacheEntity].Add(new ParticleSpawnElement
            {
                mSpawnPosition = transform.Position,
                mSpawnRotation = transform.Rotation
            });
        }
    }
}
