using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct PlayerShieldAspect : IAspect
{
    public readonly Entity mEntity;

    private readonly RefRW<LocalTransform> m_transform;

    private readonly RefRO<PlayerShieldTransform> m_shieldTransform;

    public void UpdateTransform()
    {
        m_transform.ValueRW.Position = m_shieldTransform.ValueRO.mPlayerShieldPosition;
        m_transform.ValueRW.Rotation = m_shieldTransform.ValueRO.mPlayerShieldRotation;
    }
}
