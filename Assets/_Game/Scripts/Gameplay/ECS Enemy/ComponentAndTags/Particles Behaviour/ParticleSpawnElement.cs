using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[InternalBufferCapacity(16)]
public struct ParticleSpawnElement : IBufferElementData
{
    public float3 mSpawnPosition;
    public quaternion mSpawnRotation;
}
