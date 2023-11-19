using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrainingWrapper : MonoBehaviour
{
    [SerializeField] private PlayerController m_playerController;

    private Vector3 m_playerinitialPosition;

    public bool IsPlayerLaserInput { set => m_playerController.IsPlayerLaserInput = value; }

    private void Start()
    {
        m_playerinitialPosition = m_playerController.transform.position;
    }

    public void EnqueuePlayerMovementInput(float input)
    {
        m_playerController.EnqueueMovementInput(input);
    }

    public void ResetPlayerPosition()
    {
        m_playerController.transform.position = m_playerinitialPosition;
    }
}
