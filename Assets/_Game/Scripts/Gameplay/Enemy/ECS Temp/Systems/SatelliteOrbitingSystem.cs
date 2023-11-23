using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.Search;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(SatelliteSpawnerSystem))]
public partial struct SatelliteOrbitingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SatelliteData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach ((RefRO<SatelliteData> satelliteData, Entity satelliteEntity) in 
                 SystemAPI.Query<RefRO<SatelliteData>>().WithEntityAccess())
        {
            SatelliteAspect satelliteAspect = SystemAPI.GetAspect<SatelliteAspect>(satelliteEntity);

            OrbitAspect orbitAspect = SystemAPI.GetAspect<OrbitAspect>(satelliteAspect.TargetOrbit);

            satelliteAspect.CurrentAngle += orbitAspect.SatelliteSpeed * deltaTime;
            satelliteAspect.CurrentAngle %= 360f;

            float xPos = (orbitAspect.SemiMajorAxis + satelliteAspect.SpawnOffset.x) 
                         * math.cos(math.radians(satelliteAspect.CurrentAngle));
            float yPos = (orbitAspect.SemiMinorAxis + satelliteAspect.SpawnOffset.y) 
                         * math.sin(math.radians(satelliteAspect.CurrentAngle));

            float3 targetPosition = new float3(xPos, yPos, satelliteAspect.SpawnOffset.z) *
                                    orbitAspect.OrbitThicknessRange;

            quaternion positionRotation = quaternion.AxisAngle(angle: math.acos(math.clamp(math.dot(math.normalize(math.forward()), math.normalize(orbitAspect.OrbitNormal)), -1f, 1f)),
                axis: math.normalize(math.cross(math.forward(), orbitAspect.OrbitNormal)));

            float3 rotatedPosition = orbitAspect.Transform.Position + math.mul(positionRotation.value, targetPosition);

            float3 smoothedPosition = math.lerp(satelliteAspect.Transform.Position, rotatedPosition, deltaTime);

            float3 lookDirection = math.normalize(math.cross(orbitAspect.Transform.Position - smoothedPosition,
                orbitAspect.OrbitNormal));

            float3 upDirection = math.normalize((orbitAspect.Transform.Position - smoothedPosition));

            quaternion rotation = quaternion.LookRotation(lookDirection, upDirection);
            
            LocalTransform finalTransform = new LocalTransform
            {
                Position = smoothedPosition,
                Rotation = rotation,
                Scale = satelliteAspect.Transform.Scale
            };

            commandBuffer.SetComponent(satelliteEntity, finalTransform);
        }
        
        commandBuffer.Playback(state.EntityManager);
    }
}