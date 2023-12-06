using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct FlockProperties : IComponentData
{
    public Entity? mFlockSpawner;

    public int mFlockSize;

    public float mBirdSpeed;
    public float mBirdAttackRange;

    public int mRocketsPerPatrol;
    public float mFireRateTime;
    
    public float mSeparationRadius;
}
