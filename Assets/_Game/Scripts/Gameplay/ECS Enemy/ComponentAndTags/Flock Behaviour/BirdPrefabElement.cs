using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[InternalBufferCapacity(4)]
public struct BirdPrefabElement : IBufferElementData
{
    public static implicit operator BirdPrefabElement(Entity BirdPrefab)
    {
        return new BirdPrefabElement { mBirdPrefab = BirdPrefab };
    }

    public Entity mBirdPrefab;
}
