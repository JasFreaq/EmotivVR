using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BirdData : IComponentData
{
    public Entity mOwningFlock;
    
    public bool mMarkedToDestroy;
}
