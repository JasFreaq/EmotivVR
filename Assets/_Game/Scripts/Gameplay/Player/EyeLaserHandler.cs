using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;
using static UnityEngine.Rendering.DebugUI;

public class EyeLaserHandler : MonoBehaviour
{
    [SerializeField] private VolumetricLineBehavior m_laserLine;
    [SerializeField] private VolumetricLineBehavior m_auraLine;

    [SerializeField] private float[] m_laserLengthWindows;
    [SerializeField] private float m_auraMaxWidth = 20f;

    public bool IsLaserFullyScaled => Math.Abs(m_laserLine.EndPos.z - m_laserLengthWindows[^1]) < Mathf.Epsilon;

    public float LaserRange => m_laserLine.EndPos.z;

    public void SetLaserScaling(int lengthIndex, float lengthRatio, float widthRatio)
    {
        float length = Mathf.Lerp(m_laserLengthWindows[lengthIndex], m_laserLengthWindows[lengthIndex + 1],
            lengthRatio);

        m_laserLine.EndPos = new Vector3(0f, 0f, length);

        float width = Mathf.Lerp(0f, m_auraMaxWidth, widthRatio);
        m_auraLine.LineWidth = width;
    }
}
