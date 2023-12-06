using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Transform m_eyeLaserTransform;
    [SerializeField] private Transform m_swordTransform;
    [SerializeField] private Transform m_shieldTransform;

    [SerializeField] private int m_playerInitialHealth = 200;
    [SerializeField] private PlayerHUDHandler m_playerHUD;

    private Transform m_mainCameraTransform;

    private PlayerController m_playerController;

    private EntityManager m_entityManager;
    
    private EntityQuery m_positionQuery;
    private EntityQuery m_cameraQuery;
    
    private EntityQuery m_eyeLaserQuery;
    private EntityQuery m_swordQuery;
    private EntityQuery m_shieldQuery;
    
    private EntityQuery m_stateQuery;
    private EntityQuery m_scoreQuery;

    private bool m_addedCameraDetails;

    private int m_totalScore;

    private void Awake()
    {
        m_playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        m_mainCameraTransform = Camera.main.transform;

        m_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        m_positionQuery = m_entityManager.CreateEntityQuery(typeof(PlayerCameraTransform));
        m_cameraQuery = m_entityManager.CreateEntityQuery(typeof(PlayerCameraProperties));
        
        m_eyeLaserQuery = m_entityManager.CreateEntityQuery(typeof(PlayerEyeLaserData));
        m_swordQuery = m_entityManager.CreateEntityQuery(typeof(PlayerSwordTransform));
        m_shieldQuery = m_entityManager.CreateEntityQuery(typeof(PlayerShieldTransform));
        
        m_stateQuery = m_entityManager.CreateEntityQuery(typeof(PlayerStateData));
        m_scoreQuery = m_entityManager.CreateEntityQuery(typeof(ScoreDataElement));
    }
    
    private void Update()
    {
        if (m_positionQuery.TryGetSingleton(out PlayerCameraTransform playerCameraTransform)) 
        {
            playerCameraTransform.mPlayerCameraPosition = m_mainCameraTransform.position;
            playerCameraTransform.mPlayerCameraRotation = m_mainCameraTransform.rotation;

            m_positionQuery.SetSingleton(playerCameraTransform);
        }

        if (!m_addedCameraDetails)
        {
            if (m_cameraQuery.HasSingleton<PlayerCameraProperties>())
            {
                Camera mainCamera = Camera.main;

                PlayerCameraProperties playerCameraProperties = new PlayerCameraProperties
                {
                    mCameraHalfFOV = mainCamera.fieldOfView * 0.5f,
                    mCameraAspect = mainCamera.aspect,
                    mCameraNearClipPlane = mainCamera.nearClipPlane,
                    mCameraFarClipPlane = mainCamera.farClipPlane
                };

                m_cameraQuery.SetSingleton(playerCameraProperties);

                m_addedCameraDetails = true;
            }
        }

        if (m_eyeLaserQuery.TryGetSingleton(out PlayerEyeLaserData playerEyeLaserData))
        {
            playerEyeLaserData.mLaserPosition = m_eyeLaserTransform.position;
            playerEyeLaserData.mLaserRotation = m_eyeLaserTransform.rotation;

            playerEyeLaserData.mIsLaserActive = m_playerController.IsLaserActive;
            playerEyeLaserData.mLaserRange = m_playerController.LaserRange;

            m_eyeLaserQuery.SetSingleton(playerEyeLaserData);
        }
        
        if (m_swordQuery.TryGetSingleton(out PlayerSwordTransform playerSwordTransform))
        {
            playerSwordTransform.mPlayerSwordPosition = m_swordTransform.position;
            playerSwordTransform.mPlayerSwordRotation = m_swordTransform.rotation;

            m_swordQuery.SetSingleton(playerSwordTransform);
        }
        
        if (m_shieldQuery.TryGetSingleton(out PlayerShieldTransform playerShieldTransform))
        {
            playerShieldTransform.mPlayerShieldPosition = m_shieldTransform.position;
            playerShieldTransform.mPlayerShieldRotation = m_shieldTransform.rotation;

            m_shieldQuery.SetSingleton(playerShieldTransform);
        }

        if (m_stateQuery.TryGetSingleton(out PlayerStateData playerStateData))
        {
            float healthRatio = (float)playerStateData.mPlayerHealth / m_playerInitialHealth;
            m_playerHUD.UpdateHealth(healthRatio);
        }

        if (m_scoreQuery.TryGetSingletonBuffer(out DynamicBuffer<ScoreDataElement> scoreBuffer))
        {
            foreach (ScoreDataElement scoreDataElement in scoreBuffer)
            {
                m_totalScore += scoreDataElement.mScoreIncrement;
            }

            m_playerHUD.UpdateScore(m_totalScore);
            scoreBuffer.Clear();
        }
    }
}
