using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerEyeLaserMono : MonoBehaviour
{
    [SerializeField] private float m_capsuleInitialHeight = 1f;

    public float CapsuleInitialHeight => m_capsuleInitialHeight;
}

public class PlayerEyeLaserBaker : Baker<PlayerEyeLaserMono>
{
    public override void Bake(PlayerEyeLaserMono authoring)
    {
        Entity playerEyeLaserEntity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(playerEyeLaserEntity,new PlayerEyeLaserData
        {
            mCapsuleInitialHeight = authoring.CapsuleInitialHeight
        });
    }
}
