using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public struct OrbitProperties : IComponentData
{
    public float mSemiMajorAxis;
    public float mSemiMinorAxis;
    
    public float3 mOrbitThicknessBounds;
    public float3 mOrbitNormal;
    
    public float mSatelliteSpeed;

    public float mFireRateTime;
}
