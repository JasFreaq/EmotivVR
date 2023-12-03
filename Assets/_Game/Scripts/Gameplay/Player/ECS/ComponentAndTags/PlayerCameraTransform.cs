using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PlayerCameraTransform : IComponentData
{
    public float3 mPlayerCameraPosition;
    
    public quaternion mPlayerCameraRotation;
}
