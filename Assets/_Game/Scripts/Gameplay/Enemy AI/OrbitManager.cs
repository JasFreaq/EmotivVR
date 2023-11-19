using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using TMPro;

public class OrbitManager : MonoBehaviour
{
    [Header("Orbit")]
    [SerializeField] private float m_semiMajorAxis = 50f;
    [SerializeField] private float m_semiMinorAxis = 30f;
    [SerializeField] private Vector3Int m_orbitMemberRange = new Vector3Int(2, 2, 2);
    [SerializeField] private Vector3Int m_orbitThicknessRange = new Vector3Int(2, 2, 2);
    [SerializeField] private Vector3 m_orbitNormal = Vector3.forward;
    [SerializeField] private Vector3 m_orbitReferenceTangent = Vector3.right;

    [Header("Satellites")]
    [SerializeField] private SatelliteHandler m_satellitePrefab;
    [SerializeField] private float m_satelliteSpeed = 60f;
    [SerializeField] private int m_satelliteCount = 100;

    private SatelliteHandler[] m_satellites;
    
    bool m_beganOrbiting = false;

    private float m_currentAngle;

    private void Start()
    {
        m_satellites = new SatelliteHandler[m_satelliteCount];
        HashSet<Vector3Int> satellitePositionsMap = new HashSet<Vector3Int>();

        for (int i = 0; i < m_satelliteCount; i++)
        {
            SatelliteHandler satellite = Instantiate(m_satellitePrefab);
            
            Vector3 satelliteRandomPos = GenerateRandomPointOnEllipse(satellitePositionsMap, satellite);
            
            satellite.transform.position = satelliteRandomPos;

            m_satellites[i] = satellite;
        }
    }

    private void Update()
    {
        PerformOrbiting();
    }

    private Vector3 GenerateRandomPointOnEllipse(HashSet<Vector3Int> satellitePositionsMap, SatelliteHandler satellite)
    {
        Vector3Int randomPosition;

        do
        {
            int xOffset = Mathf.RoundToInt(m_semiMajorAxis) + Random.Range(-m_orbitMemberRange.x, m_orbitMemberRange.x + 1);
            int yOffset = Mathf.RoundToInt(m_semiMajorAxis) + Random.Range(-m_orbitMemberRange.y, m_orbitMemberRange.y + 1);
            int zOffset = Random.Range(-m_orbitMemberRange.z, m_orbitMemberRange.z + 1);

            satellite.SpawnOffset = new Vector3Int(xOffset, yOffset, zOffset);

            float angle = Random.Range(0f, 2f * Mathf.PI);

            int xPos = Mathf.RoundToInt((m_semiMajorAxis + satellite.SpawnOffset.x) * Mathf.Cos(angle));
            int yPos = Mathf.RoundToInt((m_semiMinorAxis + satellite.SpawnOffset.y) * Mathf.Sin(angle));

            randomPosition = new Vector3Int(xPos, yPos, satellite.SpawnOffset.z) * m_orbitThicknessRange;

        } while (satellitePositionsMap.Contains(randomPosition));

        satellitePositionsMap.Add(randomPosition);

        float signedAngle =
            Vector3.SignedAngle(randomPosition - transform.position, m_orbitReferenceTangent, Vector3.forward);
        signedAngle = (signedAngle + 360f) % 360f;
        satellite.InitialAngle = signedAngle;

        return transform.position + Quaternion.FromToRotation(Vector3.forward, m_orbitNormal) * randomPosition;
    }

    private void PerformOrbiting()
    {
        m_currentAngle += m_satelliteSpeed * Time.deltaTime;
        m_currentAngle %= 360f;

        foreach (SatelliteHandler satellite in m_satellites)
        {
            float angle = satellite.InitialAngle + m_currentAngle;

            int xPos = Mathf.RoundToInt((m_semiMajorAxis + satellite.SpawnOffset.x) * Mathf.Cos(Mathf.Deg2Rad * angle));
            int yPos = Mathf.RoundToInt((m_semiMinorAxis + satellite.SpawnOffset.y) * Mathf.Sin(Mathf.Deg2Rad * angle));

            Vector3Int targetPosition = new Vector3Int(xPos, yPos, satellite.SpawnOffset.z) * m_orbitThicknessRange;

            Vector3 rotatedPosition = transform.position +
                                    Quaternion.FromToRotation(Vector3.forward, m_orbitNormal) * targetPosition;

            Vector3 smoothedPosition = Vector3.Lerp(satellite.transform.position, rotatedPosition,
                m_beganOrbiting ? Time.deltaTime : 1f);
            satellite.transform.position = smoothedPosition;

            Vector3 lookDirection = Vector3.Cross(transform.position - smoothedPosition, m_orbitNormal).normalized;
            Vector3 upDirection = (transform.position - satellite.transform.position).normalized;

            satellite.transform.rotation = Quaternion.LookRotation(lookDirection, upDirection);
        }

        if (!m_beganOrbiting)
        {
            m_beganOrbiting = true;
        }
    }
}
