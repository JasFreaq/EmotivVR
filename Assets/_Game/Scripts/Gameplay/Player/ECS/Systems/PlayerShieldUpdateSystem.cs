using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(PlayerUpdateSystem))]
public partial struct PlayerShieldUpdateSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerShieldTransform>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PlayerStateData playerStateData = SystemAPI.GetSingleton<PlayerStateData>();
        if (playerStateData.mIsGamePaused)
        {
            return;
        }

        Entity shieldEntity = SystemAPI.GetSingletonEntity<PlayerShieldTransform>();
        PlayerShieldAspect shieldAspect = SystemAPI.GetAspect<PlayerShieldAspect>(shieldEntity);

        shieldAspect.UpdateTransform();
    }
}
