using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_playerSpeed = 10f;
    [SerializeField] private float m_laserTime = 3f;
    [SerializeField] private EyeLaserHandler m_eyeLaser;

    private Rigidbody m_rigidbody;

    private Queue<float> m_inputQueue = new Queue<float>();
    
    private float m_laserTimer;

    [SerializeField] private bool m_isPlayerLaserInput;

    public bool IsPlayerLaserInput { set => m_isPlayerLaserInput = value; }

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        m_laserTimer = m_isPlayerLaserInput ? Mathf.Min(m_laserTimer + Time.deltaTime, m_laserTime) : Mathf.Max(m_laserTimer - Time.deltaTime, 0f);
        
        if (m_laserTimer <= Mathf.Epsilon)
        {
            if (m_eyeLaser.gameObject.activeSelf)
            {
                m_eyeLaser.SetLaserScaling(0f);
                m_eyeLaser.gameObject.SetActive(false);
            }
        }
        else
        {
            m_eyeLaser.SetLaserScaling(m_laserTimer - m_laserTime >= Mathf.Epsilon ? m_laserTime : m_laserTimer);

            if (!m_eyeLaser.gameObject.activeSelf)
                m_eyeLaser.gameObject.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        if (m_inputQueue.Count > 0)
        {
            float input = m_inputQueue.Dequeue();
            
            m_rigidbody.velocity = Vector3.forward * m_playerSpeed * input * Time.deltaTime;
        }
        else
        {
            m_rigidbody.velocity = Vector3.forward * Mathf.Lerp(m_rigidbody.velocity.magnitude, 0f, Time.deltaTime);
        }
    }

    public void EnqueueMovementInput(float input)
    {
        m_inputQueue.Enqueue(input);
    }
}
