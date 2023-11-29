using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(4)]
public struct MissileLaserElement : IBufferElementData
{
    public static implicit operator MissileLaserElement(Entity laserPrefab)
    {
        return new MissileLaserElement { mLaserPrefab = laserPrefab };
    }

    public Entity mLaserPrefab;
}