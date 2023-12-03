using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct BirdAspect : IAspect
{
    public readonly Entity mEntity;

    private readonly RefRO<LocalTransform> m_transform;

    public LocalTransform Transform => m_transform.ValueRO;


}
