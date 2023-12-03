using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerShieldMono : MonoBehaviour { }

public class PlayerShieldBaker : Baker<PlayerShieldMono>
{
    public override void Bake(PlayerShieldMono authoring)
    {
        GetEntity(TransformUsageFlags.Dynamic);
    }
}
