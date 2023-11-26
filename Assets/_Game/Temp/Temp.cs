using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

public class Temp : MonoBehaviour
{
    
}

public class TempBaker : Baker<Temp>
{
    public override void Bake(Temp authoring)
    {
        GetEntity(TransformUsageFlags.Dynamic);
    }
}
