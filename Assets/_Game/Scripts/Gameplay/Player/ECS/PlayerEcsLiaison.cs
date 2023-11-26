using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerEcsLiaison : MonoBehaviour
{
    private EntityManager m_entityManager;
    private EntityQuery m_entityQuery;
    
    private void Start()
    {
        m_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        m_entityQuery = m_entityManager.CreateEntityQuery(typeof(PlayerPositionData));
    }
    
    private void Update()
    {
        if (m_entityQuery.TryGetSingleton(out PlayerPositionData playerPositionData)) 
        {
            playerPositionData.mPlayerPosition = transform.position;

            m_entityQuery.SetSingleton(playerPositionData);
        }
    }
}
