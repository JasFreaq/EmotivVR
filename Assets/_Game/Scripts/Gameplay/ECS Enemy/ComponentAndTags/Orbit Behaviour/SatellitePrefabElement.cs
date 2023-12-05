using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[InternalBufferCapacity(4)]
public struct SatellitePrefabElement : IBufferElementData
{
    public static implicit operator SatellitePrefabElement(Entity satellitePrefab)
    {
        return new SatellitePrefabElement { mSatellitePrefab = satellitePrefab };
    }

    public Entity mSatellitePrefab;
}
