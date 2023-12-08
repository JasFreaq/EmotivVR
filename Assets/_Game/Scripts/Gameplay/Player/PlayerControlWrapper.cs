using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlWrapper : MonoBehaviour
{
    [SerializeField] private PlayerController m_playerController;

    private Vector3 m_playerInitialPosition;

    public bool IsPlayerLaserInput { set => m_playerController.IsPlayerLaserInput = value; }

    private void Start()
    {
        m_playerInitialPosition = m_playerController.transform.position;
    }

    public void EnqueuePlayerMovementInput(float input)
    {
        m_playerController.EnqueueMovementInput(input);
    }

    public void HandleProfileLoaded()
    {
        m_playerController.HandleProfileLoad();
    }

    public void ResetPlayerPosition()
    {
        m_playerController.transform.position = m_playerInitialPosition;
    }
}
