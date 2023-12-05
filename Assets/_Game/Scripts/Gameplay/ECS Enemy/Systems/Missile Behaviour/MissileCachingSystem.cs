using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct MissileCachingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MissileCacheData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity enemyElementsCacheEntity = SystemAPI.GetSingletonEntity<EnemyElementsCache>();
        
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        
        foreach((RefRO<MissileCacheData> missileCacheData, Entity missileCacheDataEntity) in 
                SystemAPI.Query<RefRO<MissileCacheData>>().WithEntityAccess())
        {
            switch (missileCacheData.ValueRO.mMissileType)
            {
                case MissileType.Laser:
                    DynamicBuffer<MissileLaserElement> laserBuffer = SystemAPI.GetBuffer<MissileLaserElement>(enemyElementsCacheEntity);
                    laserBuffer.Add(missileCacheData.ValueRO.mPrefab);
                    break;

                case MissileType.Rocket:
                    DynamicBuffer<MissileRocketElement> rocketBuffer = SystemAPI.GetBuffer<MissileRocketElement>(enemyElementsCacheEntity);
                    rocketBuffer.Add(missileCacheData.ValueRO.mPrefab);
                    break;
            }

            commandBuffer.DestroyEntity(missileCacheDataEntity);
        }

        commandBuffer.Playback(state.EntityManager);

        state.Enabled = false;
    }
}
