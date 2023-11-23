using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
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

        float spawnTimeCounter = 0f;

        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        yield return waitForEndOfFrame;

        while (generationTimer - Mathf.Epsilon <= totalGenerationTime)
        {
            generationTimer += Time.deltaTime;
            spawnTimeCounter += satellitePerUnitTime * Time.deltaTime;

            HashSet<Vector3Int> satellitePositionsMap = new HashSet<Vector3Int>();
            List<SatelliteHandler> newSatellites = new List<SatelliteHandler>();
            
            int shipsToGenerate = Mathf.FloorToInt(spawnTimeCounter);
            spawnTimeCounter -= shipsToGenerate;

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
        Vector3Int offsets = new Vector3Int();

        do
        {
            offsets.x = Random.Range(-m_orbitMemberRange.x, m_orbitMemberRange.x + 1);
            offsets.y = Random.Range(-m_orbitMemberRange.y, m_orbitMemberRange.y + 1);
            offsets.z = Random.Range(-m_orbitMemberRange.z, m_orbitMemberRange.z + 1);
            
        } while (satellitePositionsMap.Contains(offsets));

        satellite.SpawnOffset = offsets;
        satellitePositionsMap.Add(offsets);

        int xPos = Mathf.RoundToInt(m_semiMajorAxis) + offsets.x;

        Vector3Int randomPosition = new Vector3Int(xPos, 0, offsets.z) * m_orbitThicknessRange;
        
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
