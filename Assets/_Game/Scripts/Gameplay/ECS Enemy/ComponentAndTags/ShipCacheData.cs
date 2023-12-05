using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct ShipCacheData : IComponentData
{
    public Entity mPrefab;

    public ShipType mShipType;
}

public enum ShipType
{
    Satellite,
    Bird
}