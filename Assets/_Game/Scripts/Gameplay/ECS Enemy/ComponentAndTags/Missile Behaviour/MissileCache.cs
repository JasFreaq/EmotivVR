using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct MissileCache : IComponentData
{
    public float mLaserLifetime;
    public float mLaserSpeed;
    public int mLaserDamage;
    
    public float mRocketLifetime;
    public float mRocketSpeed;
    public int mRocketDamage;

    public int mShipDamage;
}
