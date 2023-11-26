using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

[BurstCompile]
[UpdateAfter(typeof(FlockFlightSystem))]
public partial struct FlockFollowingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BirdData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerPositionData>();

        PlayerAspect playerAspect = SystemAPI.GetAspect<PlayerAspect>(playerEntity);

        new FlockFollowingJob
        {
            mPlayerPosition = playerAspect.PlayerPosition

        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct FlockFollowingJob : IJobEntity
{
    public float3 mPlayerPosition;

    [BurstCompile]
    public void Execute(ref LocalTransform transform, in FlockFollower flockFollower, ref FlockUpdateData flockUpdate, ref RandomUtility random)
    {

        if (flockUpdate.mClosestBirdDistance - float.Epsilon <= flockFollower.mBirdsProximityForUpdate) 
        {
            float3 updatedPosition;

            do
            {
                float elevation = math.acos(random.mRand.NextFloat(0, 2) - 1);
                float azimuth = random.mRand.NextFloat(0, math.PI * 2);
                Debug.Log($"Azi: {azimuth} Ele: {elevation}");
                float x = flockFollower.mFollowRadius * math.sin(elevation) * math.cos(azimuth);
                float y = flockFollower.mFollowRadius * math.sin(elevation) * math.sin(azimuth);
                float z = flockFollower.mFollowRadius * math.cos(elevation);

                updatedPosition = mPlayerPosition + new float3(x, y, z);

            } while (math.distance(updatedPosition, transform.Position) <
                     flockFollower.mNewDestinationInvalidityRadius);

            flockUpdate.mClosestBirdDistance = float.MaxValue;
                transform.Position = updatedPosition;
        }
    }
}