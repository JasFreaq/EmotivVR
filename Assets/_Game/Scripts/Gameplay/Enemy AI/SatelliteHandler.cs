using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteHandler : MonoBehaviour
{
    private Vector3Int m_spawnOffset;
    
    private float m_initialAngle;

    public Vector3Int SpawnOffset
    {
        get => m_spawnOffset;
        set => m_spawnOffset = value;
    }

    public float InitialAngle
    {
        get => m_initialAngle;
        set => m_initialAngle = value;
    }
}
