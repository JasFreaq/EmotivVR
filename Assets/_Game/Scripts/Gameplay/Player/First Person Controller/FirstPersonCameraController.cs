using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCameraController : MonoBehaviour
{
    [SerializeField] private Transform m_followTarget;
    [SerializeField] private float m_zDistance = 2.5f;

    private void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, m_zDistance));
        
        m_followTarget.position = worldPosition;
    }
}
