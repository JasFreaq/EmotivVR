using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerEcsLiaison : MonoBehaviour
{
    [SerializeField] private Transform m_eyeLaserTransform;

    private PlayerController m_playerController;

    private EntityManager m_entityManager;
    private EntityQuery m_positionQuery;
    private EntityQuery m_cameraQuery;
    private EntityQuery m_eyeLaserQuery;

    private bool m_addedCameraDetails;

    private void Awake()
    {
        m_playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        m_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        m_positionQuery = m_entityManager.CreateEntityQuery(typeof(PlayerTransformData));
        m_cameraQuery = m_entityManager.CreateEntityQuery(typeof(PlayerCameraProperties));
        m_eyeLaserQuery = m_entityManager.CreateEntityQuery(typeof(PlayerEyeLaserData));
    }
    
    private void Update()
    {
        if (m_positionQuery.TryGetSingleton(out PlayerTransformData playerTransformData)) 
        {
            playerTransformData.mPlayerPosition = transform.position;
            playerTransformData.mPlayerRotation = transform.rotation;

            m_positionQuery.SetSingleton(playerTransformData);
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
    }
}
