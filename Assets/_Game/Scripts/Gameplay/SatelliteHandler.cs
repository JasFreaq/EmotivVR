using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteHandler : MonoBehaviour
{
    private Vector3Int m_spawnOffset;
    
    private float m_initialAngle;
    
    private float m_currentAngle;
    
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
    
    public float CurrentAngle
    {
        get => m_currentAngle;
        set => m_currentAngle = value;
    }
}
