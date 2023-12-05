using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ShipCacheDataMono : MonoBehaviour
{
    [SerializeField] private GameObject m_shipPrefab;
    [SerializeField] private ShipType m_shipType;

    public GameObject ShipPrefab => m_shipPrefab;

    public ShipType ShipType => m_shipType;
}

public class ShipCacheDataBaker : Baker<ShipCacheDataMono>
{
    public override void Bake(ShipCacheDataMono authoring)
    {
        Entity missileCacheDataEntity = GetEntity(TransformUsageFlags.None);

        AddComponent(missileCacheDataEntity, new ShipCacheData
        {
            mPrefab = GetEntity(authoring.ShipPrefab, TransformUsageFlags.Dynamic),
            mShipType = authoring.ShipType
        });
    }
}