using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrainingWrapper : MonoBehaviour
{
    [SerializeField] private PlayerController m_playerController;

    private Vector3 m_playerinitialPosition;

    private void Start()
    {
        m_playerinitialPosition = m_playerController.transform.position;
    }

    public void EnqueuePlayerInput(float input)
    {
        m_playerController.EnqueueInput(input);
    }

    public void ResetPlayerPosition()
    {
        m_playerController.transform.position = m_playerinitialPosition;
    }
}
