using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(PlayerUpdateSystem))]
public partial struct PlayerEyeLaserUpdateSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerEyeLaserData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerTransformData>();
        PlayerAspect playerAspect = SystemAPI.GetAspect<PlayerAspect>(playerEntity);
        
        Entity eyeLaserEntity = SystemAPI.GetSingletonEntity<PlayerEyeLaserData>();
        PlayerEyeLaserAspect eyeLaserAspect = SystemAPI.GetAspect<PlayerEyeLaserAspect>(eyeLaserEntity);

        eyeLaserAspect.UpdateTransform();
        eyeLaserAspect.UpdateCollider();
    }
}
