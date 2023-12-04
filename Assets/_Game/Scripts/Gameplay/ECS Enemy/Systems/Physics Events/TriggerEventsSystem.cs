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
        Entity missileCacheEntity = SystemAPI.GetSingletonEntity<EnemyElementsCache>();
        EnemyElementsCacheAspect enemyElementsCacheAspect = SystemAPI.GetAspect<EnemyElementsCacheAspect>(missileCacheEntity);

        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerCameraTransform>();

        TriggerEventsJob triggerJob = new TriggerEventsJob
        {
            mSatelliteScore = enemyElementsCacheAspect.SatelliteScore,
            mBirdScore = enemyElementsCacheAspect.BirdScore,
            mPlayerEntity = playerEntity,
            mPlayerEyeLaserLookup = SystemAPI.GetComponentLookup<PlayerEyeLaserData>(true),
            mPlayerSwordLookup = SystemAPI.GetComponentLookup<PlayerSwordTransform>(true),
            mPlayerShieldLookup = SystemAPI.GetComponentLookup<PlayerShieldTransform>(true),
            mPlayerScoreLookup = SystemAPI.GetBufferLookup<ScoreDataElement>(),
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
    public int mSatelliteScore;

    public int mBirdScore;

    public Entity mPlayerEntity;

    [ReadOnly]
    public ComponentLookup<PlayerEyeLaserData> mPlayerEyeLaserLookup;
    
    [ReadOnly]
    public ComponentLookup<PlayerSwordTransform> mPlayerSwordLookup;

    [ReadOnly]
    public ComponentLookup<PlayerShieldTransform> mPlayerShieldLookup;

    [NativeDisableParallelForRestriction]
    public BufferLookup<ScoreDataElement> mPlayerScoreLookup;

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
        
        if (mPlayerEyeLaserLookup.HasComponent(triggerEvent.EntityA) ||
            mPlayerSwordLookup.HasComponent(triggerEvent.EntityA) || mPlayerShieldLookup.HasComponent(triggerEvent.EntityA))
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
        if (isPlayerEntityEyeLaser && !mPlayerEyeLaserLookup.GetRefRO(playerEntity).ValueRO.mIsLaserActive)
        {
            return;
        }


        HandlePlayerTrigger(otherEntity);
    }

    private void HandlePlayerTrigger(Entity otherEntity)
    {
        int score = 0;

        if (mSatelliteLookup.HasComponent(otherEntity))
        {
            mSatelliteLookup.GetRefRW(otherEntity).ValueRW.mMarkedToDestroy = true;
            score += mSatelliteScore;
        }
        else if (mBirdLookup.HasComponent(otherEntity))
        {
            mBirdLookup.GetRefRW(otherEntity).ValueRW.mMarkedToDestroy = true;
            score += mBirdScore;
        }
        else if (mLaserLookup.HasComponent(otherEntity))
        {
            mLaserLookup.GetRefRW(otherEntity).ValueRW.mMarkedToDestroy = true;
        }
        else if (mRocketLookup.HasComponent(otherEntity))
        {
            mRocketLookup.GetRefRW(otherEntity).ValueRW.mMarkedToDestroy = true;
        }

        if (score != 0 && mPlayerScoreLookup.HasBuffer(mPlayerEntity))
        {
            DynamicBuffer<ScoreDataElement> scoreBuffer = mPlayerScoreLookup[mPlayerEntity];

            scoreBuffer.Add(score);
        }
    }
}
