using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(PlayerUpdateSystem))]
public partial struct PlayerSwordUpdateSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerStateData>();
        state.RequireForUpdate<PlayerSwordTransform>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PlayerStateData playerStateData = SystemAPI.GetSingleton<PlayerStateData>();
        if (playerStateData.mIsGamePaused)
        {
            return;
        }

        Entity swordEntity = SystemAPI.GetSingletonEntity<PlayerSwordTransform>();
        PlayerSwordAspect swordAspect = SystemAPI.GetAspect<PlayerSwordAspect>(swordEntity);

        swordAspect.UpdateTransform();
    }
}
