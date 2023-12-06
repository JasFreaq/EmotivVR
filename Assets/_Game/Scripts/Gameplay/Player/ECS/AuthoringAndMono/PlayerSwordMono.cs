using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerSwordMono : MonoBehaviour { }

public class PlayerSwordBaker : Baker<PlayerSwordMono>
{
    public override void Bake(PlayerSwordMono authoring)
    {
        Entity playerSwordEntity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent<PlayerSwordTransform>(playerSwordEntity);
    }
}
