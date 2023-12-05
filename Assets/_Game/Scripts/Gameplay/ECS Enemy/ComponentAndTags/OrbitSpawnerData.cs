using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct OrbitSpawnerData : IComponentData
{
    public Entity mOrbitTransformPrefab;
    public float3 mOrbitSpawnVolume;
    public float2 mSemiMajorAxisRange;
    public float2 mSemiMinorAxisRange;
    public int3 mMinOrbitMemberHalfBounds;
    public int3 mMaxOrbitMemberHalfBounds;
    public float3 mMinOrbitThicknessBounds;
    public float3 mMaxOrbitThicknessBounds;
    
    public Entity mSatellitePrefab;
    public float2 mSatelliteSpeedRange;
    public int2 mSatelliteCountRange;

    public int2 mLasersFiredPerSecondRange;

    public float2 mFollowSpeedRange;
}