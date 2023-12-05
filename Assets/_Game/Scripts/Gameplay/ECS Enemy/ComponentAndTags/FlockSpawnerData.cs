using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct FlockSpawnerData : IComponentData
{
    public Entity mFlockTransformPrefab;
    public float3 mFlockSpawnVolume;
    public int2 mFlockSizeRange;
    public float3 mMinFlockSpawnBounds;
    public float3 mMaxFlockSpawnBounds;
    public float3 mMinFlockSpreadRange;
    public float3 mMaxFlockSpreadRange;
    public float2 mSeparationRadiusRange;
    
    public float2 mBirdSpeedRange;
    public float2 mBirdAttackRange;
    
    public int2 mRocketsFiredPerPatrolRange;
    public float2 mRocketsFiredPerSecondRange;
    
    public float2 mFollowRadiusRange;
    public float2 mBirdsProximityForUpdateRange;
    public float2 mNewDestinationInvalidityRadiusRange;
}
