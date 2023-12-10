using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct MissileSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerStateData>();
        state.RequireForUpdate<EnemyElementsCache>();
        state.RequireForUpdate<MissileSpawnElement>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PlayerStateData playerStateData = SystemAPI.GetSingleton<PlayerStateData>();
        if (playerStateData.mIsGamePaused)
        {
            return;
        }

        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        DynamicBuffer<MissileSpawnElement> buffer = SystemAPI.GetSingletonBuffer<MissileSpawnElement>();

        Entity missileCacheEntity = SystemAPI.GetSingletonEntity<EnemyElementsCache>();
        EnemyElementsCacheAspect enemyElementsCacheAspect = SystemAPI.GetAspect<EnemyElementsCacheAspect>(missileCacheEntity);

        for (int i = 0, l = buffer.Length; i < l; i++)
        {
            buffer = SystemAPI.GetSingletonBuffer<MissileSpawnElement>();

            MissileSpawnElement missileSpawn = buffer[i];

            Entity missileEntity = new Entity();

            switch (missileSpawn.mMissileType)
            {
                case MissileType.Laser:
                    missileEntity = commandBuffer.Instantiate(enemyElementsCacheAspect.GetRandomLaser());
                    LaserData laserData = new LaserData
                    {
                        mLifetimeCounter = enemyElementsCacheAspect.LaserLifetime
                    };
                    commandBuffer.AddComponent(missileEntity, laserData);
                    break;

                case MissileType.Rocket:
                    missileEntity = commandBuffer.Instantiate(enemyElementsCacheAspect.GetRandomRocket());
                    RocketData rocketData = new RocketData
                    {
                        mLifetimeCounter = enemyElementsCacheAspect.RocketLifetime
                    };
                    commandBuffer.AddComponent(missileEntity, rocketData);
                    break;
            }

            LocalTransform spawnTransform = new LocalTransform
            {
                Position = missileSpawn.mSpawnPosition,
                Rotation = missileSpawn.mSpawnRotation,
                Scale = 1f
            };
            commandBuffer.SetComponent(missileEntity, spawnTransform);
        }

        buffer = SystemAPI.GetSingletonBuffer<MissileSpawnElement>();
        buffer.Clear();

        commandBuffer.Playback(state.EntityManager);
    }
}
