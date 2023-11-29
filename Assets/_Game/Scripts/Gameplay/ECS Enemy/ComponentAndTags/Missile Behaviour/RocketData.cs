using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct RocketData : IComponentData
{
    public float mLifetimeCounter;

    public bool mMarkedToDestroy;
}
