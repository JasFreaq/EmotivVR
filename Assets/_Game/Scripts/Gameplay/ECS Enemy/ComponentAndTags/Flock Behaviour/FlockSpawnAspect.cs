using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public readonly partial struct FlockSpawnAspect : IAspect
{
    private readonly RefRO<FlockSpawnData> m_flockSpawnData;

    private readonly RefRW<RandomUtility> m_randomUtility;
    
    public Entity BirdPrefab => m_flockSpawnData.ValueRO.mBirdPrefab;

    public float3 GetRandomOffset()
    {
        float3 range = m_flockSpawnData.ValueRO.mFlockSpawnBounds * 0.5f;
        float3 offset = m_randomUtility.ValueRW.mRand.NextFloat3(-range, range) *
                        m_flockSpawnData.ValueRO.mFlockSpreadRange;
        
        return offset;
    }
}
