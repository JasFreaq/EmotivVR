using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct ShipCachingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ShipCacheData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity enemyElementsCacheEntity = SystemAPI.GetSingletonEntity<EnemyElementsCache>();

        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach ((RefRO<ShipCacheData> shipCacheData, Entity shipCacheDataEntity) in
                 SystemAPI.Query<RefRO<ShipCacheData>>().WithEntityAccess())
        {
            switch (shipCacheData.ValueRO.mShipType)
            {
                case ShipType.Satellite:
                    DynamicBuffer<SatellitePrefabElement> satelliteBuffer = SystemAPI.GetBuffer<SatellitePrefabElement>(enemyElementsCacheEntity);
                    satelliteBuffer.Add(shipCacheData.ValueRO.mPrefab);
                    break;

                case ShipType.Bird:
                    DynamicBuffer<BirdPrefabElement> birdBuffer = SystemAPI.GetBuffer<BirdPrefabElement>(enemyElementsCacheEntity);
                    birdBuffer.Add(shipCacheData.ValueRO.mPrefab);
                    break;
            }

            commandBuffer.DestroyEntity(shipCacheDataEntity);
        }

        commandBuffer.Playback(state.EntityManager);

        state.Enabled = false;
    }
}
