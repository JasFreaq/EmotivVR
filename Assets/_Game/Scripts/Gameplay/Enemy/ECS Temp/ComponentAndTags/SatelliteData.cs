using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SatelliteData : IComponentData
{
    public float3 mSpawnOffset;

    public float mCurrentAngle;
}
