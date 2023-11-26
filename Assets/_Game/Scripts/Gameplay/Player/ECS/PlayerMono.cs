using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerMono : MonoBehaviour
{

}

public class PlayerBaker : Baker<PlayerMono>
{
    public override void Bake(PlayerMono authoring)
    {
        Entity playerEntity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent<PlayerPositionData>(playerEntity);
    }
}
