using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;


public class MissileCacheDataMono : MonoBehaviour
{
    [SerializeField] private GameObject m_missilePrefab;
    [SerializeField] private MissileType m_missileType;
    
    public GameObject MissilePrefab => m_missilePrefab;

    public MissileType MissileType => m_missileType;
}

public class MissileCacheDataBaker : Baker<MissileCacheDataMono>
{
    public override void Bake(MissileCacheDataMono authoring)
    {
        Entity missileCacheDataEntity = GetEntity(TransformUsageFlags.None);

        AddComponent(missileCacheDataEntity, new MissileCacheData
        {
            mAddedToBuffer = false,
            mPrefab = GetEntity(authoring.MissilePrefab, TransformUsageFlags.Dynamic),
            mMissileType = authoring.MissileType
        });
    }
}