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
[UpdateAfter(typeof(SatelliteOrbitingSystem))]
public partial struct LaserLocomotionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<LaserData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity missileCacheEntity = SystemAPI.GetSingletonEntity<MissileRandomUtility>();
        MissileCacheAspect missileCacheAspect = SystemAPI.GetAspect<MissileCacheAspect>(missileCacheEntity);

        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        LaserLocomotionJob laserLocomotionJob = new LaserLocomotionJob
        {
            mDeltaTime = SystemAPI.Time.DeltaTime,
            mSpeed = missileCacheAspect.LaserSpeed,
            mParallelCommandBuffer = commandBuffer.AsParallelWriter()
        };

        state.Dependency = laserLocomotionJob.ScheduleParallel(state.Dependency);
        state.Dependency.Complete();

        commandBuffer.Playback(state.EntityManager);
        commandBuffer.Dispose();
    }
}

[BurstCompile]
public partial struct LaserLocomotionJob : IJobEntity
{
    public float mDeltaTime;

    public float mSpeed;

    public EntityCommandBuffer.ParallelWriter mParallelCommandBuffer;

    [BurstCompile]
    public void Execute(ref LocalTransform transform, [ChunkIndexInQuery] int sortKey, in Entity entity, ref LaserData laserData)
    {
        laserData.mLifetimeCounter -= mDeltaTime;
        if (laserData.mLifetimeCounter > 0f)
        {
            float3 direction = transform.Forward();
            
            float3 updatedPosition = transform.Position + direction * mSpeed * mDeltaTime;

            transform.Position = updatedPosition;
        }
        else
        {
            mParallelCommandBuffer.DestroyEntity(sortKey, entity);
        }
    }
}
