using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial struct TriggerEventsSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerEyeLaserData>();
        state.RequireForUpdate<SimulationSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        TriggerEventsJob triggerJob = new TriggerEventsJob
        {
            mPlayerEyeLaserLookup = SystemAPI.GetComponentLookup<PlayerEyeLaserData>(true),
            mPlayerSwordLookup = SystemAPI.GetComponentLookup<PlayerSwordTransform>(true),
            mSatelliteLookup = SystemAPI.GetComponentLookup<SatelliteData>(),
            mBirdLookup = SystemAPI.GetComponentLookup<BirdData>(),
            mLaserLookup = SystemAPI.GetComponentLookup<LaserData>(),
            mRocketLookup = SystemAPI.GetComponentLookup<RocketData>()
        };

        state.Dependency = triggerJob.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }
}

[BurstCompile]
public struct TriggerEventsJob : ITriggerEventsJob
{
    [ReadOnly]
    public ComponentLookup<PlayerEyeLaserData> mPlayerEyeLaserLookup;
    
    [ReadOnly]
    public ComponentLookup<PlayerSwordTransform> mPlayerSwordLookup;

    [NativeDisableParallelForRestriction]
    public ComponentLookup<SatelliteData> mSatelliteLookup;

    [NativeDisableParallelForRestriction]
    public ComponentLookup<BirdData> mBirdLookup;

    [NativeDisableParallelForRestriction]
    public ComponentLookup<LaserData> mLaserLookup;

    [NativeDisableParallelForRestriction]
    public ComponentLookup<RocketData> mRocketLookup;

    public void Execute(TriggerEvent triggerEvent)
    {
        Entity playerEntity;
        Entity otherEntity;
        
        if (mPlayerEyeLaserLookup.HasComponent(triggerEvent.EntityA) /*||
            mPlayerSwordLookup.HasComponent(triggerEvent.EntityA)*/)
        {
            playerEntity = triggerEvent.EntityA;
            otherEntity = triggerEvent.EntityB;
        }
        else
        {
            playerEntity = triggerEvent.EntityB;
            otherEntity = triggerEvent.EntityA;
        }

        bool isPlayerEntityEyeLaser = mPlayerEyeLaserLookup.HasComponent(playerEntity);
        //Debug.Log(isPlayerEntityEyeLaser && !mPlayerEyeLaserLookup.GetRefRO(playerEntity).ValueRO.mIsLaserActive);
        //if (isPlayerEntityEyeLaser && !mPlayerEyeLaserLookup.GetRefRO(playerEntity).ValueRO.mIsLaserActive)
        //{
        //    return;
        //}
        Debug.Log(isPlayerEntityEyeLaser);
        if (mSatelliteLookup.HasComponent(otherEntity))
        {
            Debug.Log("Satellite");
            mSatelliteLookup.GetRefRW(otherEntity).ValueRW.mMarkedToDestroy = true;
        }
        else if (mBirdLookup.HasComponent(otherEntity))
        {
            Debug.Log("Bird");
            mBirdLookup.GetRefRW(otherEntity).ValueRW.mMarkedToDestroy = true;
        }
        else if (mLaserLookup.HasComponent(otherEntity))
        {
            Debug.Log("Laser");
            mLaserLookup.GetRefRW(otherEntity).ValueRW.mMarkedToDestroy = true;
        }
        else if (mRocketLookup.HasComponent(otherEntity))
        {
            Debug.Log("Rocket");
            mRocketLookup.GetRefRW(otherEntity).ValueRW.mMarkedToDestroy = true;
        }
    }
}
