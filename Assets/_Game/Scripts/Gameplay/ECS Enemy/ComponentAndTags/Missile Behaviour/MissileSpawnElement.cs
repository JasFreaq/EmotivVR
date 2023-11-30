using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[InternalBufferCapacity(16)]
public struct MissileSpawnElement : IBufferElementData
{
    public float3 mSpawnPosition;
    public quaternion mSpawnRotation;

    public MissileType mMissileType;
}
