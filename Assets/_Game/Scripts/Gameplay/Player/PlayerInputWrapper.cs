using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputWrapper : MonoBehaviour
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
}
