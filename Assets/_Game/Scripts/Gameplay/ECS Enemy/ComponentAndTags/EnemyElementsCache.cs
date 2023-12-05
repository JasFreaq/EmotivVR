using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct EnemyElementsCache : IComponentData
{
    public float mLaserLifetime;
    public float mLaserSpeed;
    public int mLaserDamage;
    
    public float mRocketLifetime;
    public float mRocketSpeed;
    public int mRocketDamage;

    public int mShipDamage;

    public int mSatelliteScore;
    public int mBirdScore;
}
