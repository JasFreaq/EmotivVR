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
[UpdateAfter(typeof(FlockSpawnerSystem))]
public partial struct FlockAssignmentSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BirdData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRO<BirdData> birdData, Entity birdEntity) in
                 SystemAPI.Query<RefRO<BirdData>>().WithEntityAccess())
        {
            FlockAspect flockAspect = SystemAPI.GetAspect<FlockAspect>(birdData.ValueRO.mOwningFlock);

            FixedList512Bytes<Entity> flockMembers = flockAspect.Birds;

            flockMembers.Add(birdEntity);

            flockAspect.Birds = flockMembers;
        }

        state.Enabled = false;
    }
}
