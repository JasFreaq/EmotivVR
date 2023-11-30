using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.VisualScripting;
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

        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerTransformData>();
        PlayerAspect playerAspect = SystemAPI.GetAspect<PlayerAspect>(playerEntity);

        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        FlockFlightJob job = new FlockFlightJob
        {
            mDeltaTime = SystemAPI.Time.DeltaTime,
            mTime = (float)SystemAPI.Time.ElapsedTime,
            mPlayerPosition = playerAspect.Transform.Position,
            mMissileCacheEntity = SystemAPI.GetSingletonEntity<MissileCache>(),
            mParticlesCacheEntity = SystemAPI.GetSingletonEntity<ParticlesCache>(),
            mLocalToWorldLookup = state.GetComponentLookup<LocalToWorld>(true),
            mFlockPropertiesLookup = state.GetComponentLookup<FlockProperties>(true),
            mBirdLookup = state.GetBufferLookup<FlockBirdElement>(),
            mFlockUpdateDataLookup = state.GetComponentLookup<FlockUpdateData>(),
            mMissileSpawnBuffer = SystemAPI.GetBufferLookup<MissileSpawnElement>(),
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
    
    [ReadOnly]
    public float3 mPlayerPosition;

    public Entity mMissileCacheEntity;
    
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
    public BufferLookup<MissileSpawnElement> mMissileSpawnBuffer;
    
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
                DynamicBuffer<FlockBirdElement> birdBuffer = mBirdLookup[birdData.mOwningFlock];

                int birdIndex = -1;
                for (int i = 0, l = birdBuffer.Length; i < l; i++)
                {
                    if (entity == birdBuffer[i].mBird)
                    {
                        birdIndex = i;
                        break;
                    }
                }

                birdBuffer.RemoveAt(birdIndex);
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
                SpawnRockets(transform, flockProperties.mFireRateTime, flockUpdate, flockProperties);
            }
        }
    }

    [BurstCompile]
    private void SpawnRockets(LocalTransform transform, float fireRateTime, RefRW<FlockUpdateData> flockUpdate, FlockProperties flockProperties)
    {
        float lastFireTime = flockUpdate.ValueRO.mLastFireTime;
        float fireTimer = flockUpdate.ValueRO.mFireTimer;

        if (fireTimer - lastFireTime >= fireRateTime &&
            flockUpdate.ValueRO.mRocketsFired < flockProperties.mRocketsPerPatrol)
        {
            if (mMissileSpawnBuffer.HasBuffer(mMissileCacheEntity))
            {
                mMissileSpawnBuffer[mMissileCacheEntity].Add(new MissileSpawnElement
                {
                    mSpawnPosition = transform.Position,
                    mSpawnRotation = transform.Rotation,
                    mMissileType = MissileType.Rocket
                });

                flockUpdate.ValueRW.mRocketsFired++;
                flockUpdate.ValueRW.mLastFireTime = mTime;
            }

            //LocalTransform spawnTransform = new LocalTransform
            //{
            //    Position = transform.Position,
            //    Rotation = transform.Rotation,
            //    Scale = 1f
            //};
            //mParallelCommandBuffer.SetComponent(sortKey, rocketEntity, spawnTransform);

            //RocketData rocketData = new RocketData
            //{
            //    mLifetimeCounter = mRocketLifetime
            //};
            //mParallelCommandBuffer.AddComponent(sortKey, rocketEntity, rocketData);
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