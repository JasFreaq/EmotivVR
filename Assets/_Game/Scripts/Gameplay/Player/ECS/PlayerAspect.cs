using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct PlayerAspect : IAspect
{
    public readonly Entity mEntity;

    private readonly RefRW<LocalTransform> m_transform;

    private readonly RefRO<PlayerPositionData> m_playerPositionData;

    public float3 PlayerPosition => m_transform.ValueRO.Position;

    public void UpdatePosition()
    {
        m_transform.ValueRW.Position = m_playerPositionData.ValueRO.mPlayerPosition;
    }
}
