using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static UnityEngine.GraphicsBuffer;

[BurstCompile]
[RequireMatchingQueriesForUpdate]
[UpdateAfter(typeof(SatelliteSpawnerSystem))]
public partial class SatelliteOrbitingSystem : SystemBase
{
    private EntityQuery m_query;
    
    [BurstCompile]
    protected override void OnCreate()
    {
        RequireForUpdate<SatelliteData>();

        m_query = GetEntityQuery
        (
            typeof(LocalTransform),
            ComponentType.ReadWrite<SatelliteData>()
        );
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        SatelliteOrbitingJob job = new SatelliteOrbitingJob
        {
            mLocalToWorldLookup = GetComponentLookup<LocalToWorld>(true),
            mOrbitPropertiesLookup = GetComponentLookup<OrbitProperties>(true),
            mDeltaTime = SystemAPI.Time.DeltaTime
        };

        Dependency = job.ScheduleParallel(m_query, Dependency);
    }
}

[BurstCompile]
public partial struct SatelliteOrbitingJob : IJobEntity
{
    [ReadOnly]
    public ComponentLookup<LocalToWorld> mLocalToWorldLookup;

    [ReadOnly]
    public ComponentLookup<OrbitProperties> mOrbitPropertiesLookup;
    
    public float mDeltaTime;

    [BurstCompile]
    public void Execute(ref LocalTransform transform, ref SatelliteData satelliteData)
    {
        

        if (mOrbitPropertiesLookup.TryGetComponent(satelliteData.mTargetOrbit, out OrbitProperties orbitProperties))
        {
            if (!mLocalToWorldLookup.HasComponent(satelliteData.mTargetOrbit))
                return;

            float3 orbitPosition = mLocalToWorldLookup.GetRefRO(satelliteData.mTargetOrbit).ValueRO.Position;

            satelliteData.mCurrentAngle += orbitProperties.mSatelliteSpeed * mDeltaTime;
            satelliteData.mCurrentAngle %= 360f;

            float xPos = (orbitProperties.mSemiMajorAxis + satelliteData.mSpawnOffset.x)
                         * math.cos(math.radians(satelliteData.mCurrentAngle));
            float yPos = (orbitProperties.mSemiMinorAxis + satelliteData.mSpawnOffset.y)
                         * math.sin(math.radians(satelliteData.mCurrentAngle));

            float3 targetPosition = new float3(xPos, yPos, satelliteData.mSpawnOffset.z) *
                                    orbitProperties.mOrbitThicknessBounds;

            quaternion positionRotation = quaternion.AxisAngle(angle: math.acos(math.clamp(math.dot(math.normalize(math.forward()), math.normalize(orbitProperties.mOrbitNormal)), -1f, 1f)),
                axis: math.normalize(math.cross(math.forward(), orbitProperties.mOrbitNormal)));

            float3 rotatedPosition = orbitPosition + math.mul(positionRotation.value, targetPosition);

            float3 smoothedPosition = math.lerp(transform.Position, rotatedPosition, mDeltaTime);

            float3 lookDirection = math.normalize(math.cross(orbitPosition - smoothedPosition, orbitProperties.mOrbitNormal));

            float3 upDirection = math.normalize(orbitPosition - smoothedPosition);

            quaternion rotation = quaternion.LookRotation(lookDirection, upDirection);

            transform.Position = smoothedPosition;
            transform.Rotation = rotation;
        }
    }
}