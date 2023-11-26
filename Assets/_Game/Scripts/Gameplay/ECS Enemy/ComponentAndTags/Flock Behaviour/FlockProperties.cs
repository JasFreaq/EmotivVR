using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct FlockProperties : IComponentData
{
    public int mFlockSize;
    public float mFlockSpeed;
    
    public float mSeparationRadius;
    public float mSeekWeight;
}
