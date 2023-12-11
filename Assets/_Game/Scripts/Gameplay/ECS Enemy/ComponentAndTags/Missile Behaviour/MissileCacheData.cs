using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct MissileCacheData : IComponentData
{
    public bool mAddedToBuffer;

    public Entity mPrefab;

    public MissileType mMissileType;
}

public enum MissileType
{
    Laser,
    Rocket
}