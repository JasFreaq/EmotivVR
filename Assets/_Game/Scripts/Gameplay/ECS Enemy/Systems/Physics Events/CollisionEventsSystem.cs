using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial struct CollisionEventsSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MissileCache>();
        state.RequireForUpdate<PlayerHealthData>();
        state.RequireForUpdate<SimulationSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity missileCacheEntity = SystemAPI.GetSingletonEntity<MissileCache>();
        MissileCacheAspect missileCacheAspect = SystemAPI.GetAspect<MissileCacheAspect>(missileCacheEntity);

        Entity particlesCacheEntity = SystemAPI.GetSingletonEntity<ParticlesCache>();
        ParticlesCacheAspect particlesCacheAspect = SystemAPI.GetAspect<ParticlesCacheAspect>(particlesCacheEntity);
        
        CollisionEventsJob collisionJob = new CollisionEventsJob
        {
            mLaserDamage = missileCacheAspect.LaserDamage,
            mRocketDamage = missileCacheAspect.RocketDamage,
            mShipDamage = missileCacheAspect.ShipDamage,
            mExplosionLifetime = particlesCacheAspect.TinyExplosionLifetime,
            mExplosionParticles = particlesCacheAspect.TinyExplosion,
            mPlayerHealthLookup = SystemAPI.GetComponentLookup<PlayerHealthData>(),
            mSatelliteLookup = SystemAPI.GetComponentLookup<SatelliteData>(),
            mBirdLookup = SystemAPI.GetComponentLookup<BirdData>(),
            mLaserLookup = SystemAPI.GetComponentLookup<LaserData>(),
            mRocketLookup = SystemAPI.GetComponentLookup<RocketData>()
        };

        state.Dependency = collisionJob.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }
}

[BurstCompile]
public struct CollisionEventsJob : ICollisionEventsJob
{
    public int mLaserDamage;
    
    public int mRocketDamage;
    
    public int mShipDamage;

    public float mExplosionLifetime;
    
    public Entity mExplosionParticles;

    [NativeDisableParallelForRestriction]
    public ComponentLookup<PlayerHealthData> mPlayerHealthLookup;

    [NativeDisableParallelForRestriction]
    public ComponentLookup<SatelliteData> mSatelliteLookup;

    [NativeDisableParallelForRestriction]
    public ComponentLookup<BirdData> mBirdLookup;
    
    [NativeDisableParallelForRestriction]
    public ComponentLookup<LaserData> mLaserLookup;

    [NativeDisableParallelForRestriction]
    public ComponentLookup<RocketData> mRocketLookup;

    [BurstCompile]
    public void Execute(CollisionEvent collisionEvent)
    {
        Entity playerEntity = new Entity();
        Entity otherEntity;

        if (mPlayerHealthLookup.HasComponent(collisionEvent.EntityA))
        {
            playerEntity = collisionEvent.EntityA;
            otherEntity = collisionEvent.EntityB;
        }
        else 
        {
            playerEntity = collisionEvent.EntityB;
            otherEntity = collisionEvent.EntityA;
        }

        HandlePlayerCollision(playerEntity, otherEntity);
    }

    private void HandlePlayerCollision(Entity playerEntity, Entity otherEntity)
    {
        int damage = 0;
        if (mSatelliteLookup.HasComponent(otherEntity))
        {
            damage = mShipDamage;
            mSatelliteLookup.GetRefRW(otherEntity).ValueRW.mMarkedToDestroy = true;
        }
        else if (mBirdLookup.HasComponent(otherEntity))
        {
            damage = mShipDamage;
            mBirdLookup.GetRefRW(otherEntity).ValueRW.mMarkedToDestroy = true;
        }
        else if (mLaserLookup.HasComponent(otherEntity))
        {
            damage = mLaserDamage;
            mLaserLookup.GetRefRW(otherEntity).ValueRW.mMarkedToDestroy = true;
        }
        else if (mRocketLookup.HasComponent(otherEntity))
        {
            damage = mRocketDamage;
            mRocketLookup.GetRefRW(otherEntity).ValueRW.mMarkedToDestroy = true;
        }

        mPlayerHealthLookup.GetRefRW(playerEntity).ValueRW.mPlayerHealth -= damage;
    }
}
