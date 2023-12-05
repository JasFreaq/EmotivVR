using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct FlockSpawnerSystem : ISystem
{
    //[BurstCompile]
    //public void OnCreate(ref SystemState state)
    //{
    //    state.RequireForUpdate<OrbitProperties>();
    //}

    //[BurstCompile]
    //public void OnUpdate(ref SystemState state)
    //{
    //    foreach ((RefRO<OrbitProperties> orbitProperties, Entity orbitEntity) in
    //             SystemAPI.Query<RefRO<OrbitProperties>>().WithEntityAccess())
    //    {
            
    //    }
    //}
}
