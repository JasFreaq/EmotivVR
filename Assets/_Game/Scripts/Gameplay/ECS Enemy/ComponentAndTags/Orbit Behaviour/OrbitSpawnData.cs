using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public struct OrbitSpawnData : IComponentData
{
    public float mGenerationTimer;

    public float mTotalGenerationTime;

    public float mSatellitePerUnitTime;

    public float mSpawnTimeCounter;

    public float3 mOrbitMemberHalfBounds;

    public Entity mSatellitePrefab;

    public Random mRand;
}