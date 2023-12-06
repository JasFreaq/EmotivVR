using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(BirdSpawnerSystem))]
public partial struct BirdAssignmentSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BirdData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRW<BirdData> birdData, Entity birdEntity) in SystemAPI.Query<RefRW<BirdData>>().WithEntityAccess())
        {
            if (!birdData.ValueRO.mAssignedToFlock) 
            {
                DynamicBuffer<FlockBirdElement> flockMembers =
                    SystemAPI.GetBuffer<FlockBirdElement>(birdData.ValueRO.mOwningFlock);
                flockMembers.Add(birdEntity);

                birdData.ValueRW.mAssignedToFlock = true;
            }
        }

        state.Enabled = false;
    }
}
