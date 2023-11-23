using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct OrbitProperties : IComponentData
{
    public float mSemiMajorAxis;
    public float mSemiMinorAxis;
    public int3 mOrbitMemberRange;
    public float3 mOrbitThicknessRange;
    public float3 mOrbitNormal;
    
    public Entity mSatellitePrefab;
    public float mSatelliteSpeed;
    public int mSatelliteCount;
}
