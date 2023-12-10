using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct FlockAspect : IAspect
{
    public readonly Entity mEntity;

    private readonly RefRO<LocalTransform> m_transform;

    private readonly RefRO<FlockProperties> m_flockProperties;

    private readonly DynamicBuffer<FlockBirdElement> m_flockBirdsBuffer;

    public LocalTransform Transform => m_transform.ValueRO;

    public int FlockSize => m_flockProperties.ValueRO.mFlockSize;

    public DynamicBuffer<FlockBirdElement> FlockBirdsBuffer => m_flockBirdsBuffer;
}
