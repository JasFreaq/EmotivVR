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
        Entity missileCacheEntity = SystemAPI.GetSingletonEntity<MissileRandomUtility>();
        
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        
        foreach((RefRO<MissileCacheData> missileCacheData, Entity missileCacheDataEntity) in 
                SystemAPI.Query<RefRO<MissileCacheData>>().WithEntityAccess())
        {
            DynamicBuffer<MissileLaserElement> buffer = SystemAPI.GetBuffer<MissileLaserElement>(missileCacheEntity);
            buffer.Add(missileCacheData.ValueRO.mPrefab);

            commandBuffer.DestroyEntity(missileCacheDataEntity);
        }

        commandBuffer.Playback(state.EntityManager);

        state.Enabled = false;
    }
}
