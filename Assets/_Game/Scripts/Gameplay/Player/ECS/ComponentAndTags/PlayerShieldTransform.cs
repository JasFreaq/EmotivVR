using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PlayerShieldTransform : IComponentData
{
    public float3 mPlayerShieldPosition;

    public quaternion mPlayerShieldRotation;
}
