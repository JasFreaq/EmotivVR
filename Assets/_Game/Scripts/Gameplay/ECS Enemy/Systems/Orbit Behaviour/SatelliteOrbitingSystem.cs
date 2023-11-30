using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using JetBrains.Annotations;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct SatelliteOrbitingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SatelliteData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRW<OrbitUpdateData> orbitUpdateData in SystemAPI.Query<RefRW<OrbitUpdateData>>())
        {
            orbitUpdateData.ValueRW.mFireTimer += SystemAPI.Time.DeltaTime;
        }

        Entity missileCacheEntity = SystemAPI.GetSingletonEntity<MissileCache>();
        MissileCacheAspect missileCacheAspect = SystemAPI.GetAspect<MissileCacheAspect>(missileCacheEntity);

        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerTransformData>();
        PlayerAspect playerAspect = SystemAPI.GetAspect<PlayerAspect>(playerEntity);

        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);
        
        SatelliteOrbitingJob job = new SatelliteOrbitingJob
        {
            mDeltaTime = SystemAPI.Time.DeltaTime,
            mTime = (float)SystemAPI.Time.ElapsedTime,
            mMissileCacheEntity = SystemAPI.GetSingletonEntity<MissileCache>(),
            mParticlesCacheEntity = SystemAPI.GetSingletonEntity<ParticlesCache>(),
            mPlayerTransform = playerAspect.Transform,
            mCameraProperties = playerAspect.CameraProperties,
            mLocalToWorldLookup = state.GetComponentLookup<LocalToWorld>(true),
            mOrbitPropertiesLookup = state.GetComponentLookup<OrbitProperties>(true),
            mOrbitUpdateDataLookup = state.GetComponentLookup<OrbitUpdateData>(),
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
public partial struct SatelliteOrbitingJob : IJobEntity
{
    public float mDeltaTime;

    public float mTime;
    
    public Entity mMissileCacheEntity;

    public Entity mParticlesCacheEntity;

    [ReadOnly] 
    public LocalTransform mPlayerTransform;

    [ReadOnly]
    public PlayerCameraProperties mCameraProperties;
    
    [ReadOnly]
    public ComponentLookup<LocalToWorld> mLocalToWorldLookup;

    [ReadOnly]
    public ComponentLookup<OrbitProperties> mOrbitPropertiesLookup;

    [NativeDisableParallelForRestriction]
    public ComponentLookup<OrbitUpdateData> mOrbitUpdateDataLookup;

    [NativeDisableParallelForRestriction]
    public BufferLookup<MissileSpawnElement> mMissileSpawnBuffer;

    [NativeDisableParallelForRestriction]
    public BufferLookup<ParticleSpawnElement> mParticleSpawnBuffer;

    public EntityCommandBuffer.ParallelWriter mParallelCommandBuffer;

    [BurstCompile]
    public void Execute(ref LocalTransform transform, [ChunkIndexInQuery] int sortKey, in Entity entity, ref SatelliteData satelliteData)
    {
        if (satelliteData.mMarkedToDestroy)
        {
            if (mOrbitUpdateDataLookup.HasComponent(satelliteData.mTargetOrbit))
            {
                mOrbitUpdateDataLookup.GetRefRW(satelliteData.mTargetOrbit).ValueRW.mOrbitSatelliteCount--;
            }

            SpawnExplosion(transform);
            mParallelCommandBuffer.DestroyEntity(sortKey, entity);
            return;
        }

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

            quaternion positionRotation = quaternion.AxisAngle(angle: math.acos(math.clamp(math.dot(math.normalizesafe(math.forward()), math.normalizesafe(orbitProperties.mOrbitNormal)), -1f, 1f)),
                axis: math.normalizesafe(math.cross(math.forward(), orbitProperties.mOrbitNormal)));

            float3 rotatedPosition = orbitPosition + math.mul(positionRotation.value, targetPosition);

            float3 smoothedPosition = math.lerp(transform.Position, rotatedPosition, mDeltaTime);

            float3 lookDirection = math.normalizesafe(math.cross(orbitPosition - smoothedPosition, orbitProperties.mOrbitNormal));

            float3 upDirection = math.normalizesafe(orbitPosition - smoothedPosition);
            
            transform.Position = smoothedPosition;
            transform.Rotation = quaternion.LookRotation(lookDirection, upDirection);

            if (IsSatelliteInView(transform))
            {
                if (!mOrbitUpdateDataLookup.HasComponent(satelliteData.mTargetOrbit))
                    return;

                RefRW<OrbitUpdateData> orbitUpdate = mOrbitUpdateDataLookup.GetRefRW(satelliteData.mTargetOrbit);

                SpawnLasers(orbitProperties.mFireRateTime, orbitUpdate, smoothedPosition, upDirection, lookDirection);
            }
        }
    }

    [BurstCompile]
    private bool IsSatelliteInView(LocalTransform transform)
    {
        float3 relativePosition = mPlayerTransform.InverseTransformPoint(transform.Position);

        float distance = relativePosition.z;

        float frustumHeight = 2.0f * distance * Mathf.Tan(Mathf.Deg2Rad * mCameraProperties.mCameraHalfFOV);
        float frustumWidth = frustumHeight * mCameraProperties.mCameraAspect;

        if (Mathf.Abs(relativePosition.x) < frustumWidth * 0.5f && Mathf.Abs(relativePosition.y) < frustumHeight * 0.5f &&
            distance > mCameraProperties.mCameraNearClipPlane && distance < mCameraProperties.mCameraFarClipPlane)
        {
            return true;
        }

        return false;
    }

    [BurstCompile]
    private void SpawnLasers(float fireRateTime, RefRW<OrbitUpdateData> orbitUpdate, float3 smoothedPosition, float3 upDirection, float3 lookDirection)
    {
        float lastFireTime = orbitUpdate.ValueRO.mLastFireTime;
        float fireTimer = orbitUpdate.ValueRO.mFireTimer;

        if (fireTimer - lastFireTime >= fireRateTime)
        {
            float fireChance = orbitUpdate.ValueRW.mRand.NextFloat();
            int satelliteCount = orbitUpdate.ValueRO.mOrbitSatelliteCount;

            if (fireChance < 1f / satelliteCount)
            {
                if (mMissileSpawnBuffer.HasBuffer(mMissileCacheEntity))
                {
                    mMissileSpawnBuffer[mMissileCacheEntity].Add(new MissileSpawnElement
                    {
                        mSpawnPosition = smoothedPosition,
                        mSpawnRotation = quaternion.LookRotation(upDirection, lookDirection),
                        mMissileType = MissileType.Laser
                    });

                    orbitUpdate.ValueRW.mLastFireTime = mTime;
                }
            }
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