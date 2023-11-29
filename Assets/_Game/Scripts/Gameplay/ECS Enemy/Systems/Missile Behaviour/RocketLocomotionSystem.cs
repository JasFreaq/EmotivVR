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
        Entity missileCacheEntity = SystemAPI.GetSingletonEntity<MissileRandomUtility>();
        MissileCacheAspect missileCacheAspect = SystemAPI.GetAspect<MissileCacheAspect>(missileCacheEntity);

        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerTransformData>();
        PlayerAspect playerAspect = SystemAPI.GetAspect<PlayerAspect>(playerEntity);

        Entity particlesCacheEntity = SystemAPI.GetSingletonEntity<ParticlesCache>();
        ParticlesCacheAspect particlesCacheAspect = SystemAPI.GetAspect<ParticlesCacheAspect>(particlesCacheEntity);

        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        RocketLocomotionJob rocketLocomotionJob = new RocketLocomotionJob
        {
            mDeltaTime = SystemAPI.Time.DeltaTime,
            mSpeed = missileCacheAspect.RocketSpeed,
            mExplosionLifetime = particlesCacheAspect.TinyExplosionLifetime,
            mPlayerPosition = playerAspect.Transform.Position,
            mExplosionParticles = particlesCacheAspect.TinyExplosion,
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

    public float mExplosionLifetime;

    public float3 mPlayerPosition;
    
    public Entity mExplosionParticles;

    public EntityCommandBuffer.ParallelWriter mParallelCommandBuffer;
    
    [BurstCompile]
    public void Execute(ref LocalTransform transform, [ChunkIndexInQuery] int sortKey, in Entity entity,
        ref RocketData rocketData)
    {
        if (rocketData.mMarkedToDestroy)
        {
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
            Entity particlesEntity = mParallelCommandBuffer.Instantiate(sortKey, mExplosionParticles);
            
            LocalTransform spawnTransform = new LocalTransform
            {
                Position = transform.Position,
                Rotation = transform.Rotation,
                Scale = 1f
            };
            mParallelCommandBuffer.SetComponent(sortKey, particlesEntity, spawnTransform);
            
            ParticleUpdateData particleUpdate = new ParticleUpdateData
            {
                mParticleLifetimeCounter = mExplosionLifetime
            };
            mParallelCommandBuffer.AddComponent(sortKey, particlesEntity, particleUpdate);

            rocketData.mMarkedToDestroy = true;
        }
    }
}
