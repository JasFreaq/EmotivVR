using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Random = Unity.Mathematics.Random;

public struct OrbitUpdateData : IComponentData
{
    public float mFireRateTime;

    public float mLastFireTime;
    
    public float mFireTimer;

    public int mOrbitSatelliteCount;

    public Random mRand;
}
