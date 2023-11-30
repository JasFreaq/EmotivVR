using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PlayerEyeLaserData : IComponentData
{
    public float3 mLaserPosition;
    public quaternion mLaserRotation;

    public float mCapsuleInitialHeight;

    public bool mIsLaserActive;

    public float mLaserRange;
}
