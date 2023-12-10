using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(SatelliteOrbitingSystem))]
public partial struct OrbitFollowingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerStateData>();
        state.RequireForUpdate<PlayerCameraTransform>();
        state.RequireForUpdate<SatelliteData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PlayerStateData playerStateData = SystemAPI.GetSingleton<PlayerStateData>();
        if (playerStateData.mIsGamePaused)
        {
            return;
        }

        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerCameraTransform>();

        PlayerAspect playerAspect = SystemAPI.GetAspect<PlayerAspect>(playerEntity);

        new OrbitFollowingJob
        {
            mDeltaTime = SystemAPI.Time.DeltaTime,
            mPlayerPosition = playerAspect.PlayerPosition

        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct OrbitFollowingJob : IJobEntity
{
    public float mDeltaTime;

    public float3 mPlayerPosition;

    [BurstCompile]
    public void Execute(ref LocalTransform transform, in OrbitFollower orbitFollower)
    {
        float3 directionToPlayer = math.normalizesafe(mPlayerPosition - transform.Position);
        
        float3 updatedPosition = transform.Position + directionToPlayer * orbitFollower.mFollowSpeed * mDeltaTime;

        transform.Position = updatedPosition;
    }
}
