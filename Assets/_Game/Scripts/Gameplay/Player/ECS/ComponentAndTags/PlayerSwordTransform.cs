using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PlayerSwordTransform : IComponentData
{
    public float3 mPlayerSwordPosition;

    public quaternion mPlayerSwordRotation;
}
