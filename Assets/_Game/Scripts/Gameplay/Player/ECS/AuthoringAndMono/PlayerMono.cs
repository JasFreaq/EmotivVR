using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerMono : MonoBehaviour
{
    [SerializeField] private int m_playerStartingHealth = 200;

    public int PlayerStartingHealth => m_playerStartingHealth;
}

public class PlayerBaker : Baker<PlayerMono>
{
    public override void Bake(PlayerMono authoring)
    {
        Entity playerEntity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent<PlayerCameraTransform>(playerEntity);
        
        AddComponent<PlayerCameraProperties>(playerEntity);

        AddComponent(playerEntity, new PlayerStateData
        {
            mPlayerHealth = authoring.PlayerStartingHealth
        });

        AddComponent<ScoreDataElement>(playerEntity);
    }
}
