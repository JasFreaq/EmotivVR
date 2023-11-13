using System;
using System.Collections;
using EmotivUnityPlugin;
using EmotivVR;
using System.Collections.Generic;
using UnityEngine;

public class EmotivTrainingInterfacer : MonoBehaviour
{
    [SerializeField] private ApplicationConfiguration m_appConfig;

    [SerializeField] private TrainingUIHandler m_trainingUI;

    private EmotivUnityItf m_emotivInterface = EmotivUnityItf.Instance;

    private DataStreamManager m_dataStreamManager = DataStreamManager.Instance;

    private const float k_TimeUpdateData = 1f;

    private const bool k_IsDataBufferUsing = false; // default subscribed data will not saved to Data buffer

    private const string k_ProfileName = "Default"; // default profile name for playtesting

    private float m_timerDataUpdate = 0;

    private bool m_isAuthorized;

    private bool m_isSessionCreated;

    private Queue<SystemEvent> m_commandQueue = new Queue<SystemEvent>();

    private void Start()
    {
        m_emotivInterface.Init(m_appConfig.ClientId, m_appConfig.ClientSecret, m_appConfig.AppName, m_appConfig.AppVersion,
            k_IsDataBufferUsing);
        
        m_emotivInterface.Start();
    }

    private void OnEnable()
    {
        m_trainingUI.SubscribeToStartButton(() =>
        {
            if (m_emotivInterface.IsProfileLoaded)
                m_emotivInterface.StartMCTraining(m_trainingUI.CurrentProfileName);
        });
        
        m_trainingUI.SubscribeToEraseButton(() =>
        {
            if (m_emotivInterface.IsProfileLoaded)
                m_emotivInterface.EraseMCTraining(m_trainingUI.CurrentProfileName);
        });
        
        m_trainingUI.SubscribeToCancelButton(() =>
        {
            if (m_emotivInterface.IsProfileLoaded)
                m_emotivInterface.RejectMCTraining();
        });
        
        m_trainingUI.SubscribeToAcceptButton(() =>
        {
            if (m_emotivInterface.IsProfileLoaded)
                m_emotivInterface.AcceptMCTraining();
        });
        
        m_trainingUI.SubscribeToRejectButton(() =>
        {
            if (m_emotivInterface.IsProfileLoaded)
                m_emotivInterface.RejectMCTraining();
        });
    }

    private void Update()
    {
        m_timerDataUpdate += Time.deltaTime;
        
        if (m_timerDataUpdate - k_TimeUpdateData >= Mathf.Epsilon)
        {
            m_timerDataUpdate -= k_TimeUpdateData;

            if (!m_emotivInterface.IsSessionCreated)
            {
                //TODO: Add training splash screen with 'create session' button. Link CreateSession to said button and Enable said button here
                m_emotivInterface.CreateSessionWithHeadset("");
            }

            if (!m_isAuthorized && m_emotivInterface.IsAuthorizedOK)
            {
                m_trainingUI.EnableLoadProfileButton();

                m_isAuthorized = true;
            }

            if (!m_isSessionCreated && m_emotivInterface.IsSessionCreated)
            {
                m_emotivInterface.SubscribeData(new List<string> { "sys" });
                m_dataStreamManager.SysEventsReceived += OnSysEventsReceived;

                m_isSessionCreated = true;
            }
        }

        if (m_commandQueue.Count > 0) 
        {
            ProcessSystemEvent(m_commandQueue.Dequeue());
        }
    }
    
    private void OnDisable()
    {
        m_trainingUI.UnsubscribeEventsFromAllButtons();
    }

    public void LoadProfile()
    {
        m_emotivInterface.LoadProfile(k_ProfileName);
    }
    
    private void OnSysEventsReceived(object sender, SysEventArgs data)
    {
        if (Enum.TryParse(data.EventMessage, out SystemEvent mentalEvent))
        {
            m_commandQueue.Enqueue(mentalEvent);
        }
        else
        {
            Debug.LogError("Invalid Event received from SysEvent stream.");
        }
    }

    private void ProcessSystemEvent(SystemEvent mentalEvent)
    {
        switch (mentalEvent)
        {
            case SystemEvent.MC_Started:
                m_trainingUI.SetTrainingState();
                break;

            case SystemEvent.MC_Succeeded:
                m_trainingUI.SetTrainedState();
                break;

            case SystemEvent.MC_Completed:
                m_trainingUI.SetBaseState();
                break;

            case SystemEvent.MC_Rejected:
                m_trainingUI.SetBaseState();
                break;
        }
    }
}
