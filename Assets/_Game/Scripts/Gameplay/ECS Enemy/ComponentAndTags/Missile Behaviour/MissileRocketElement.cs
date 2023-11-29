using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[InternalBufferCapacity(2)]
public struct MissileRocketElement : IBufferElementData
{
    public static implicit operator MissileRocketElement(Entity rocketPrefab)
    {
        return new MissileRocketElement { mRocketPrefab = rocketPrefab };
    }

    public Entity mRocketPrefab;
}
