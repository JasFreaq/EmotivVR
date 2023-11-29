using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct LaserData : IComponentData
{
    public float mLifetimeCounter;

    public bool mMarkedToDestroy;
}
