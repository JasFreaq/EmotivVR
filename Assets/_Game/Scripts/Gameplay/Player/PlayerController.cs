using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_playerSpeed = 50f;
    [SerializeField] private float[] m_laserTimes;
    [SerializeField] private EyeLaserHandler m_eyeLaser;

    [SerializeField] private InputActionReference m_pauseInputAction;
    [SerializeField] private bool m_isPlayMode;
    
    private PlayerStateManager m_playerStateManager;

    private Transform m_cameraTransform;

    private Rigidbody m_rigidbody;

    private AudioSource m_flyAudioSource;

    private Queue<float> m_inputQueue = new Queue<float>();

    private float m_totalLaserTime;

    private float m_laserTimer;

    private bool m_isPlayerLaserInput;

    private bool m_isProfileLoaded;
    
    private bool m_isGamePaused;
    
    public bool IsPlayerLaserInput { set => m_isPlayerLaserInput = value; }

    public bool IsLaserActive => m_eyeLaser.gameObject.activeSelf;
    
    public float LaserRange => m_eyeLaser.LaserRange;

    private void Awake()
    {
        m_playerStateManager = GetComponent<PlayerStateManager>();

        if (m_isPlayMode)
            m_playerStateManager.PauseGame(true, false);

        m_rigidbody = GetComponent<Rigidbody>();
        m_flyAudioSource = GetComponent<AudioSource>();
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
        if (!m_isProfileLoaded)
            return;

        if (m_pauseInputAction != null) 
        {
            if (m_pauseInputAction.action.WasPressedThisFrame())
            {
                PauseGame();
            }
        }

        if (m_isGamePaused)
        {
            return;
        }

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
        if (m_isGamePaused)
            return;

        if (m_inputQueue.Count > 0)
        {
            float input = m_inputQueue.Dequeue();
            
            m_rigidbody.velocity = m_cameraTransform.forward * m_playerSpeed * input * Time.deltaTime;

            if (!m_flyAudioSource.isPlaying) 
                m_flyAudioSource.Play();
        }
        else
        {
            m_rigidbody.velocity = m_cameraTransform.forward * Mathf.Lerp(m_rigidbody.velocity.magnitude, 0f, Time.deltaTime);

            if (m_flyAudioSource.isPlaying)
                m_flyAudioSource.Stop();
        }
    }

    public void EnqueueMovementInput(float input)
    {
        if (m_isGamePaused)
            return;

        m_inputQueue.Enqueue(input);
    }

    public void HandleProfileLoad()
    {
        m_isProfileLoaded = true;
        m_playerStateManager.PauseGame(false, false);
    }

    public void PauseGame()
    {
        m_isGamePaused = !m_isGamePaused;
        m_playerStateManager.PauseGame(m_isGamePaused);
    }
}
