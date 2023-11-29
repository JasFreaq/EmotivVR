using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public struct FlockSpawnData : IComponentData
{
    public float3 mFlockSpawnBounds;
    
    public float3 mFlockSpreadRange;

    public Entity mBirdPrefab;

    public Random mRand;
}
