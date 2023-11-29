using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;


public class MissileCacheDataMono : MonoBehaviour
{
    [SerializeField] private GameObject m_laserPrefab;
    [SerializeField] private MissileType m_missileType;
    
    public GameObject LaserPrefab => m_laserPrefab;

    public MissileType MissileType => m_missileType;
}

public class MissileCacheDataBaker : Baker<MissileCacheDataMono>
{
    public override void Bake(MissileCacheDataMono authoring)
    {
        Entity missileCacheDataEntity = GetEntity(TransformUsageFlags.None);

        AddComponent(missileCacheDataEntity, new MissileCacheData
        {
            mPrefab = GetEntity(authoring.LaserPrefab, TransformUsageFlags.Dynamic),
            mMissileType = authoring.MissileType
        });
    }
}