using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSystemGroup))]
public partial struct FlockFlightSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BirdData>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRW<FlockUpdateData> flockUpdateData in SystemAPI.Query<RefRW<FlockUpdateData>>())
        {
            flockUpdateData.ValueRW.mFireTimer += SystemAPI.Time.DeltaTime;
        }

        Entity missileCacheEntity = SystemAPI.GetSingletonEntity<MissileCache>();
        MissileCacheAspect missileCacheAspect = SystemAPI.GetAspect<MissileCacheAspect>(missileCacheEntity);

        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerTransformData>();
        PlayerAspect playerAspect = SystemAPI.GetAspect<PlayerAspect>(playerEntity);

        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        FlockFlightJob job = new FlockFlightJob
        {
            mDeltaTime = SystemAPI.Time.DeltaTime,
            mTime = (float)SystemAPI.Time.ElapsedTime,
            mRocketLifetime = missileCacheAspect.RocketLifetime,
            mRocketEntity = missileCacheAspect.GetRandomRocket(),
            mPlayerPosition = playerAspect.Transform.Position,
            mParticlesCacheEntity = SystemAPI.GetSingletonEntity<ParticlesCache>(),
            mLocalToWorldLookup = state.GetComponentLookup<LocalToWorld>(true),
            mFlockPropertiesLookup = state.GetComponentLookup<FlockProperties>(true),
            mBirdLookup = state.GetBufferLookup<FlockBirdElement>(),
            mFlockUpdateDataLookup = state.GetComponentLookup<FlockUpdateData>(),
            mParticleSpawnBuffer = SystemAPI.GetBufferLookup<ParticleSpawnElement>(),
            mParallelCommandBuffer = commandBuffer.AsParallelWriter()
        };
        
        state.Dependency = job.ScheduleParallel(state.Dependency);
        state.Dependency.Complete();

        commandBuffer.Playback(state.EntityManager);
        commandBuffer.Dispose();
    }
}

[BurstCompile]
public partial struct FlockFlightJob : IJobEntity
{
    public float mDeltaTime;

    public float mTime;

    public float mRocketLifetime;

    [ReadOnly]
    public float3 mPlayerPosition;

    public Entity mRocketEntity;

    public Entity mParticlesCacheEntity;

    [ReadOnly]
    public ComponentLookup<LocalToWorld> mLocalToWorldLookup;

    [ReadOnly]
    public ComponentLookup<FlockProperties> mFlockPropertiesLookup;

    [NativeDisableParallelForRestriction]
    public BufferLookup<FlockBirdElement> mBirdLookup;

    [NativeDisableParallelForRestriction]
    public ComponentLookup<FlockUpdateData> mFlockUpdateDataLookup;

    [NativeDisableParallelForRestriction]
    public BufferLookup<ParticleSpawnElement> mParticleSpawnBuffer;

    public EntityCommandBuffer.ParallelWriter mParallelCommandBuffer;

    [BurstCompile]
    public void Execute(ref LocalTransform transform, [ChunkIndexInQuery] int sortKey, in Entity entity, in BirdData birdData, ref PhysicsVelocity physicsVelocity)
    {
        if (birdData.mMarkedToDestroy)
        {
            if (mBirdLookup.HasBuffer(birdData.mOwningFlock))
            {
                mBirdLookup[birdData.mOwningFlock].RemoveAt(birdData.mBufferIndex);
            }

            SpawnExplosion(transform);
            mParallelCommandBuffer.DestroyEntity(sortKey, entity);
            return;
        }

        if (mFlockPropertiesLookup.TryGetComponent(birdData.mOwningFlock, out FlockProperties flockProperties))
        {
            if (!mFlockUpdateDataLookup.HasComponent(birdData.mOwningFlock))
                return;

            RefRW<FlockUpdateData> flockUpdate = mFlockUpdateDataLookup.GetRefRW(birdData.mOwningFlock);

            float3 targetPosition = mLocalToWorldLookup.GetRefRO(birdData.mOwningFlock).ValueRO.Position;

            float distanceToTarget = math.distance(targetPosition, transform.Position);

            if (distanceToTarget < flockUpdate.ValueRO.mClosestBirdDistance)
                flockUpdate.ValueRW.mClosestBirdDistance = distanceToTarget;
            
            DynamicBuffer<FlockBirdElement> flockMembers = mBirdLookup[birdData.mOwningFlock];
            
            float3 cohesion = float3.zero;
            float3 separation = float3.zero;
            
            float3 velocity = physicsVelocity.Linear;

            foreach (FlockBirdElement otherBirdElement in flockMembers)
            {
                if (!mLocalToWorldLookup.HasComponent(otherBirdElement.mBird))
                    return;

                float3 otherBirdPosition = mLocalToWorldLookup.GetRefRO(otherBirdElement.mBird).ValueRO.Position;
                float distance = math.distance(transform.Position, otherBirdPosition);

                if (distance <= float.Epsilon) 
                {
                    continue;
                }

                cohesion += otherBirdPosition;

                if (distance < flockProperties.mSeparationRadius)
                    separation += (transform.Position - otherBirdPosition) / (distance * distance);
            }

            cohesion /= flockProperties.mFlockSize;
            cohesion = math.normalizesafe(cohesion - transform.Position);

            velocity += cohesion + separation;

            float3 seekDirection = math.normalizesafe(targetPosition - transform.Position);
            velocity += seekDirection * flockProperties.mSeekWeight;

            physicsVelocity.Linear = math.normalizesafe(velocity)
                                             * math.min(math.length(velocity), flockProperties.mBirdSpeed);

            quaternion rotation = quaternion.LookRotationSafe(math.normalizesafe(seekDirection), transform.Up());
            transform.Rotation = math.slerp(transform.Rotation, rotation, mDeltaTime);
            
            if (math.distance(transform.Position, mPlayerPosition) <= flockProperties.mBirdAttackRange) 
            {
                SpawnRockets(transform, flockProperties.mFireRateTime, sortKey, flockUpdate, flockProperties);
            }
        }
    }

    [BurstCompile]
    private void SpawnRockets(LocalTransform transform, float fireRateTime, int sortKey, RefRW<FlockUpdateData> flockUpdate, FlockProperties flockProperties)
    {
        float lastFireTime = flockUpdate.ValueRO.mLastFireTime;
        float fireTimer = flockUpdate.ValueRO.mFireTimer;

        if (fireTimer - lastFireTime >= fireRateTime &&
            flockUpdate.ValueRO.mRocketsFired < flockProperties.mRocketsPerPatrol)
        {
            Entity rocketEntity = mParallelCommandBuffer.Instantiate(sortKey, mRocketEntity);

            LocalTransform spawnTransform = new LocalTransform
            {
                Position = transform.Position,
                Rotation = transform.Rotation,
                Scale = 1f
            };
            mParallelCommandBuffer.SetComponent(sortKey, rocketEntity, spawnTransform);

            RocketData rocketData = new RocketData
            {
                mLifetimeCounter = mRocketLifetime
            };
            mParallelCommandBuffer.AddComponent(sortKey, rocketEntity, rocketData);

            flockUpdate.ValueRW.mRocketsFired++;
            flockUpdate.ValueRW.mLastFireTime = mTime;
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