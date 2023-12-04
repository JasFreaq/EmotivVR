using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_playerSpeed = 50f;
    [SerializeField] private float[] m_laserTimes;
    [SerializeField] private EyeLaserHandler m_eyeLaser;

    private Transform m_cameraTransform;

    private Rigidbody m_rigidbody;

    private Queue<float> m_inputQueue = new Queue<float>();

    private float m_totalLaserTime;

    private float m_laserTimer;

    private bool m_isPlayerLaserInput;

    public bool IsPlayerLaserInput { set => m_isPlayerLaserInput = value; }

    public bool IsLaserActive => m_eyeLaser.gameObject.activeSelf;
    
    public float LaserRange => m_eyeLaser.LaserRange;
    
    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        m_cameraTransform = Camera.main.transform;

        foreach (float time in m_laserTimes)
        {
            m_totalLaserTime += time;
        }
    }

    private void Update()
    {
        m_laserTimer += Time.deltaTime * (m_isPlayerLaserInput ? 1f : -1f);
        m_laserTimer = Mathf.Clamp(m_laserTimer, 0f, m_totalLaserTime);
        
        if (m_laserTimer <= Mathf.Epsilon)
        {
            if (m_eyeLaser.gameObject.activeSelf)
            {
                m_eyeLaser.SetLaserScaling(0, 0f, 0f);
                m_eyeLaser.gameObject.SetActive(false);
            }
        }
        else if (m_laserTimer - Mathf.Epsilon < m_totalLaserTime)
        {
            float upper = 0f, timeSpent = 0;
            int l = m_laserTimes.Length;

            for (int i = 0; i < l; i++)
            {
                float lower = i == 0 ? 0f : m_laserTimes[i - 1];
                upper += m_laserTimes[i];

                if (m_laserTimer - lower >= Mathf.Epsilon &&
                    m_laserTimer - upper <= Mathf.Epsilon)
                {
                    m_eyeLaser.SetLaserScaling(i, (m_laserTimer - timeSpent) / m_laserTimes[i],
                        m_laserTimer / m_totalLaserTime);

                    break;
                }

                timeSpent = upper;
            }

            if (!m_eyeLaser.gameObject.activeSelf)
                m_eyeLaser.gameObject.SetActive(true);
        }
        else if (!m_eyeLaser.IsLaserFullyScaled)
        {
            m_eyeLaser.SetLaserScaling(m_laserTimes.Length - 1, 1f, 1f);
        }
    }

    private void FixedUpdate()
    {
        if (m_inputQueue.Count > 0)
        {
            float input = m_inputQueue.Dequeue();
            
            m_rigidbody.velocity = m_cameraTransform.forward * m_playerSpeed * input * Time.deltaTime;
        }
        else
        {
            m_rigidbody.velocity = m_cameraTransform.forward * Mathf.Lerp(m_rigidbody.velocity.magnitude, 0f, Time.deltaTime);
        }
    }

    public void EnqueueMovementInput(float input)
    {
        m_inputQueue.Enqueue(input);
    }
}
