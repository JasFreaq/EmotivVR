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
public partial class FlockFlightSystem : SystemBase
{
    private EntityQuery m_query;

    [BurstCompile]
    protected override void OnCreate()
    {
        RequireForUpdate<BirdData>();

        m_query = GetEntityQuery
        (
            typeof(LocalTransform),
            ComponentType.ReadOnly<BirdData>(),
            ComponentType.ReadWrite<PhysicsVelocity>()
        );
    }
    
    [BurstCompile]
    protected override void OnUpdate()
    {
        FlockFlightJob job = new FlockFlightJob
        {
            mDeltaTime = SystemAPI.Time.DeltaTime,
            mLocalToWorldLookup = GetComponentLookup<LocalToWorld>(true),
            mFlockPropertiesLookup = GetComponentLookup<FlockProperties>(true),
            mFlockUpdateDataLookup = GetComponentLookup<FlockUpdateData>()
        };

        Dependency = job.ScheduleParallel(m_query, Dependency);
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
    
    [NativeDisableParallelForRestriction]
    public ComponentLookup<FlockUpdateData> mFlockUpdateDataLookup;

    [BurstCompile]
    public void Execute(ref LocalTransform transform, in BirdData birdData, ref PhysicsVelocity physicsVelocity)
    {
        if (mFlockPropertiesLookup.TryGetComponent(birdData.mOwningFlock, out FlockProperties flockProperties))
        {
            if (!mFlockUpdateDataLookup.HasComponent(birdData.mOwningFlock))
                return;

            float3 targetPosition = mLocalToWorldLookup.GetRefRO(birdData.mOwningFlock).ValueRO.Position;

            float distanceToTarget = math.distance(targetPosition, transform.Position);

            if (distanceToTarget < mFlockUpdateDataLookup.GetRefRO(birdData.mOwningFlock).ValueRO.mClosestBirdDistance)
                mFlockUpdateDataLookup.GetRefRW(birdData.mOwningFlock).ValueRW.mClosestBirdDistance = distanceToTarget;

            FixedList512Bytes<Entity> flockMembers = mFlockUpdateDataLookup.GetRefRO(birdData.mOwningFlock).ValueRO.mBirds;

            float3 cohesion = float3.zero;
            float3 separation = float3.zero;
            
            float3 velocity = physicsVelocity.Linear;

            foreach (Entity otherBirdEntity in flockMembers)
            {
                if (!mLocalToWorldLookup.HasComponent(otherBirdEntity))
                    return;

                float3 otherBirdPosition = mLocalToWorldLookup.GetRefRO(otherBirdEntity).ValueRO.Position;
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