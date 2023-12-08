using EmotivUnityPlugin;
using EmotivVR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotivGameplayInterfacer : MonoBehaviour
{
    private static string s_ProfileName = "LaserKnight Profile A";

    [SerializeField] private ApplicationConfiguration m_appConfig;
    [SerializeField] private GameStartUIHandler m_gameStartUIHandler;

    private EmotivUnityItf m_emotivInterface = EmotivUnityItf.Instance;

    private PlayerControlWrapper m_playerControlWrapper;

    private const float k_TimeUpdateData = 1f;

    private const bool k_IsDataBufferUsing = false; // default subscribed data will not saved to Data buffer
    
    private float m_timerDataUpdate;
    
    private bool m_isSessionCreated;

    private bool m_isProfileLoaded;

    public static string ProfileName
    {
        get => s_ProfileName;
        set => s_ProfileName = value;
    }

    private void Awake()
    {
        m_playerControlWrapper = GetComponent<PlayerControlWrapper>();
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
            
            if (m_emotivInterface.IsSessionCreated)
            {
                if (!m_isSessionCreated)
                {
                    m_emotivInterface.SubscribeData(new List<string> { "com" });
                    m_emotivInterface.MentalCommandReceived += OnMentalCommandReceived;

                    m_gameStartUIHandler.EnableLoadProfileButton();

                    m_isSessionCreated = true;
                }
            }

            if (!m_isProfileLoaded && m_emotivInterface.IsProfileLoaded)
            {
                m_gameStartUIHandler.HandleProfileLoaded();
                m_playerControlWrapper.HandleProfileLoaded();

                m_isProfileLoaded = true;
            }
        }
    }
    
    private void OnDestroy()
    {
        m_emotivInterface.Stop();
    }

    public void CreateSession()
    {
        m_emotivInterface.CreateSessionWithHeadset("");
    }

    public void LoadProfile()
    {
        m_emotivInterface.LoadProfile(s_ProfileName);
    }

    private void OnMentalCommandReceived(MentalCommandEventArgs data)
    {
        if (data.Act == "pull")
        {
            m_playerControlWrapper.EnqueuePlayerMovementInput((float)data.Pow);
        }

        m_playerControlWrapper.IsPlayerLaserInput = data.Act == "push";
    }
}
