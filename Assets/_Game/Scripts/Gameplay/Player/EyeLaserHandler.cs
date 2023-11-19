using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

public class EyeLaserHandler : MonoBehaviour
{
    [SerializeField] private VolumetricLineBehavior m_laserLine;
    [SerializeField] private VolumetricLineBehavior m_auraLine;

    [SerializeField] private float[] m_laserLengthWindows;
    [SerializeField] private float m_auraMaxWidth = 20f;
    
    public void SetLaserScaling(float ratio)
    {
        int index = Mathf.Min(Mathf.FloorToInt(ratio), m_laserLengthWindows.Length - 2);

        float length = Mathf.Lerp(m_laserLengthWindows[index], m_laserLengthWindows[index + 1], ratio - index);
        m_laserLine.EndPos = new Vector3(0f, length, 0f);

        float width = Mathf.Lerp(0f, m_auraMaxWidth, ratio / (m_laserLengthWindows.Length - 1));
        m_auraLine.LineWidth = width;
    }
}
