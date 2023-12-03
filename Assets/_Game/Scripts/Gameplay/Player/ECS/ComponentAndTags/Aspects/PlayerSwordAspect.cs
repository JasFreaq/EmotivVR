using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct PlayerSwordAspect : IAspect
{
    public readonly Entity mEntity;

    private readonly RefRW<LocalTransform> m_transform;

    private readonly RefRO<PlayerSwordTransform> m_swordTransform;

    public void UpdateTransform()
    {
        m_transform.ValueRW.Position = m_swordTransform.ValueRO.mPlayerSwordPosition;
        m_transform.ValueRW.Rotation = m_swordTransform.ValueRO.mPlayerSwordRotation;
    }
}
