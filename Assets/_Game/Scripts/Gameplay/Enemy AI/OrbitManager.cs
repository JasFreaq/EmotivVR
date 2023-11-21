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

    [Header("Satellites")]
    [SerializeField] private SatelliteHandler m_satellitePrefab;
    [SerializeField] private float m_satelliteSpeed = 60f;
    [SerializeField] private int m_satelliteCount = 100;

    private List<SatelliteHandler> m_satellites = new List<SatelliteHandler>();
    
    bool m_beganOrbiting;
    
    private void Start()
    {
        StartCoroutine(GenerateSatellitesRoutine());
    }

    private void Update()
    {
        PerformOrbiting();
    }

    private IEnumerator GenerateSatellitesRoutine()
    {
        float generationTimer = 0f;
        
        float totalGenerationTime = 360f / m_satelliteSpeed;

        float satellitePerUnitTime = m_satelliteCount / totalGenerationTime;

        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        yield return waitForEndOfFrame;

        while (generationTimer - Mathf.Epsilon <= totalGenerationTime)
        {
            generationTimer += Time.deltaTime;

            HashSet<Vector3Int> satellitePositionsMap = new HashSet<Vector3Int>();
            List<SatelliteHandler> newSatellites = new List<SatelliteHandler>();

            int shipsToGenerate = Mathf.RoundToInt(satellitePerUnitTime * Time.deltaTime);
            int extraShips = (m_satelliteCount % shipsToGenerate) / (m_satelliteCount / shipsToGenerate);

            shipsToGenerate += extraShips;

            for (int i = 0; i < shipsToGenerate; i++)
            {
                SatelliteHandler satellite = Instantiate(m_satellitePrefab);

                Vector3 satelliteRandomPos = GenerateRandomPointOnEllipse(satellitePositionsMap, satellite);

                satellite.transform.position = satelliteRandomPos;

                newSatellites.Add(satellite);
            }

            m_satellites.AddRange(newSatellites);

            yield return waitForEndOfFrame;
        }
    }

    private Vector3 GenerateRandomPointOnEllipse(HashSet<Vector3Int> satellitePositionsMap, SatelliteHandler satellite)
    {
        Vector3Int randomPosition;

        do
        {
            int xOffset = Random.Range(-m_orbitMemberRange.x, m_orbitMemberRange.x + 1);
            int yOffset = Random.Range(-m_orbitMemberRange.y, m_orbitMemberRange.y + 1);
            int zOffset = Random.Range(-m_orbitMemberRange.z, m_orbitMemberRange.z + 1);

            satellite.SpawnOffset = new Vector3Int(xOffset, yOffset, zOffset);

            int xPos = Mathf.RoundToInt(m_semiMajorAxis) + xOffset;

            randomPosition = new Vector3Int(xPos, 0, zOffset) * m_orbitThicknessRange;

        } while (satellitePositionsMap.Contains(randomPosition));

        satellitePositionsMap.Add(randomPosition);

        return transform.position + Quaternion.FromToRotation(Vector3.forward, m_orbitNormal) * randomPosition;
    }

    private void PerformOrbiting()
    {
        foreach (SatelliteHandler satellite in m_satellites)
        {
            satellite.CurrentAngle += m_satelliteSpeed * Time.deltaTime;
            satellite.CurrentAngle %= 360f;

            int xPos = Mathf.RoundToInt((m_semiMajorAxis + satellite.SpawnOffset.x) * Mathf.Cos(Mathf.Deg2Rad * satellite.CurrentAngle));
            int yPos = Mathf.RoundToInt((m_semiMinorAxis + satellite.SpawnOffset.y) * Mathf.Sin(Mathf.Deg2Rad * satellite.CurrentAngle));

            Vector3Int targetPosition = new Vector3Int(xPos, yPos, satellite.SpawnOffset.z) * m_orbitThicknessRange;

            Vector3 rotatedPosition = transform.position +
                                    Quaternion.FromToRotation(Vector3.forward, m_orbitNormal) * targetPosition;

            Vector3 smoothedPosition = Vector3.Lerp(satellite.transform.position, rotatedPosition, Time.deltaTime);
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
