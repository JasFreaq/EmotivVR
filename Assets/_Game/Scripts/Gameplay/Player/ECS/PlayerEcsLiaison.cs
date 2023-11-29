using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerEcsLiaison : MonoBehaviour
{
    private EntityManager m_entityManager;
    private EntityQuery m_positionQuery;
    private EntityQuery m_cameraQuery;

    private bool m_addedCameraDetails;

    private void Start()
    {
        m_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        m_positionQuery = m_entityManager.CreateEntityQuery(typeof(PlayerTransformData));
        m_cameraQuery = m_entityManager.CreateEntityQuery(typeof(PlayerCameraProperties));
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
    }
}
