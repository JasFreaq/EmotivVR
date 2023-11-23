using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public struct OrbitRandom : IComponentData
{
    public Random mRand;
}
