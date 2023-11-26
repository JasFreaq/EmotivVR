using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct FlockFollower : IComponentData
{
    public float mFollowRadius;

    public float mBirdsProximityForUpdate;

    public float mNewDestinationInvalidityRadius;
}
