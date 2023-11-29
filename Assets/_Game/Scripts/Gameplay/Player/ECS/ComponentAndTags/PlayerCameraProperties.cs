using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct PlayerCameraProperties : IComponentData
{
    public float mCameraHalfFOV;

    public float mCameraAspect;

    public float mCameraNearClipPlane;
    
    public float mCameraFarClipPlane;
}
