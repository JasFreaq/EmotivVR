using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public struct FlockUpdateData : IComponentData
{
    public float mClosestBirdDistance;

    public int mRocketsFired;

    public float mLastFireTime;

    public float mFireTimer;

    public Random mRand;
}
