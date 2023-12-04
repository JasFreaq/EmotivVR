using EmotivUnityPlugin;
using EmotivVR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotivGameplayInterfacer : MonoBehaviour
{
    [SerializeField] private ApplicationConfiguration m_appConfig;
    
    private EmotivUnityItf m_emotivInterface = EmotivUnityItf.Instance;

    private PlayerInputWrapper m_playerInputWrapper;

    private const float k_TimeUpdateData = 1f;

    private const bool k_IsDataBufferUsing = false; // default subscribed data will not saved to Data buffer

    private const string k_ProfileName = "LaserKnight"; // default profile name for playtesting
    
    private float m_timerDataUpdate;

    private bool m_isAuthorized;

    private bool m_isSessionCreated;

    private bool m_isProfileLoaded;
    
    private void Awake()
    {
        m_playerInputWrapper = GetComponent<PlayerInputWrapper>();
    }

    private void Start()
    {
        m_emotivInterface.Init(m_appConfig.ClientId, m_appConfig.ClientSecret, m_appConfig.AppName, m_appConfig.AppVersion,
            k_IsDataBufferUsing);

        m_emotivInterface.Start();
    }
    
    private void Update()
    {
        m_timerDataUpdate += Time.deltaTime;

        if (m_timerDataUpdate - k_TimeUpdateData >= Mathf.Epsilon)
        {
            m_timerDataUpdate -= k_TimeUpdateData;

            if (!m_emotivInterface.IsSessionCreated)
            {
                m_emotivInterface.CreateSessionWithHeadset("");
            }

            if (!m_isAuthorized && m_emotivInterface.IsAuthorizedOK)
            {
                m_emotivInterface.LoadProfile(k_ProfileName);

                m_isAuthorized = true;
            }

            if (!m_isSessionCreated && m_emotivInterface.IsSessionCreated)
            {
                m_emotivInterface.SubscribeData(new List<string> { "com" });
                m_emotivInterface.MentalCommandReceived += OnMentalCommandReceived;
                
                m_isSessionCreated = true;
            }

            if (!m_isProfileLoaded && m_emotivInterface.IsProfileLoaded)
            {
                m_isProfileLoaded = true;
            }
        }
    }
    
    private void OnApplicationQuit()
    {
        m_emotivInterface.Stop();
    }
    
    private void OnMentalCommandReceived(MentalCommandEventArgs data)
    {
        if (data.Act == "pull")
        {
            m_playerInputWrapper.EnqueuePlayerMovementInput((float)data.Pow);
        }

        m_playerInputWrapper.IsPlayerLaserInput = data.Act == "push";
    }
}
