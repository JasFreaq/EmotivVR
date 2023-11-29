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
        FlockFlightJob job = new FlockFlightJob
        {
            mDeltaTime = SystemAPI.Time.DeltaTime,
            mLocalToWorldLookup = state.GetComponentLookup<LocalToWorld>(true),
            mFlockPropertiesLookup = state.GetComponentLookup<FlockProperties>(true),
            mBirdLookup = state.GetBufferLookup<FlockBirdElement>(true),
            mFlockUpdateDataLookup = state.GetComponentLookup<FlockUpdateData>()
        };
        
        state.Dependency = job.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
public partial struct FlockFlightJob : IJobEntity
{
    public float mDeltaTime;
    
    [ReadOnly]
    public ComponentLookup<LocalToWorld> mLocalToWorldLookup;

    [ReadOnly]
    public ComponentLookup<FlockProperties> mFlockPropertiesLookup;

    [ReadOnly]
    public BufferLookup<FlockBirdElement> mBirdLookup;

    [NativeDisableParallelForRestriction]
    public ComponentLookup<FlockUpdateData> mFlockUpdateDataLookup;

    [BurstCompile]
    public void Execute(ref LocalTransform transform, [ChunkIndexInQuery] int sortKey, in BirdData birdData, ref PhysicsVelocity physicsVelocity)
    {
        if (mFlockPropertiesLookup.TryGetComponent(birdData.mOwningFlock, out FlockProperties flockProperties))
        {
            if (!mFlockUpdateDataLookup.HasComponent(birdData.mOwningFlock))
                return;

            float3 targetPosition = mLocalToWorldLookup.GetRefRO(birdData.mOwningFlock).ValueRO.Position;

            float distanceToTarget = math.distance(targetPosition, transform.Position);

            if (distanceToTarget < mFlockUpdateDataLookup.GetRefRO(birdData.mOwningFlock).ValueRO.mClosestBirdDistance)
                mFlockUpdateDataLookup.GetRefRW(birdData.mOwningFlock).ValueRW.mClosestBirdDistance = distanceToTarget;
            
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
                                             * math.min(math.length(velocity), flockProperties.mFlockSpeed);

            quaternion rotation = quaternion.LookRotationSafe(math.normalizesafe(velocity), transform.Up());
            transform.Rotation = math.slerp(transform.Rotation, rotation, mDeltaTime);
        }
    }
}