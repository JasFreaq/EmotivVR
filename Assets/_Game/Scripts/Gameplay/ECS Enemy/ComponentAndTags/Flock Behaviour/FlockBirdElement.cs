using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[InternalBufferCapacity(64)]
public struct FlockBirdElement : IBufferElementData
{
    public static implicit operator FlockBirdElement(Entity bird)
    {
        return new FlockBirdElement { mBird = bird };
    }

    public Entity mBird;
}
