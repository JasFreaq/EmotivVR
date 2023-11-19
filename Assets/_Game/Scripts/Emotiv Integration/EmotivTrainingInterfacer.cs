using System;
using System.Collections;
using EmotivUnityPlugin;
using EmotivVR;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class EmotivTrainingInterfacer : MonoBehaviour
{
    [SerializeField] private ApplicationConfiguration m_appConfig;

    [SerializeField] private TrainingUIHandler m_trainingUI;

    [SerializeField] private string[] m_actionNames;

    private EmotivUnityItf m_emotivInterface = EmotivUnityItf.Instance;

    private PlayerTrainingWrapper m_playerTrainingWrapper;

    private const float k_TimeUpdateData = 1f;

    private const bool k_IsDataBufferUsing = false; // default subscribed data will not saved to Data buffer

    private const string k_ProfileName = "Default"; // default profile name for playtesting

    private Queue<SystemEvent> m_sysEventQueue = new Queue<SystemEvent>();

    private Queue<JObject> m_signatureActionsQueue = new Queue<JObject>();
    private Queue<JArray> m_brainMapQueue = new Queue<JArray>();
    private Queue<JObject> m_trainingThresholdQueue = new Queue<JObject>();

    private Dictionary<string, int> m_signatureActionsDict = new Dictionary<string, int>();

    private float m_timerDataUpdate;

    private bool m_isAuthorized;

    private bool m_isSessionCreated;
    
    private bool m_isProfileLoaded;

    private bool m_checkedSignatureActionOnLoad;

    private void Awake()
    {
        m_playerTrainingWrapper = GetComponent<PlayerTrainingWrapper>();
    }

    private void Start()
    {
        m_emotivInterface.Init(m_appConfig.ClientId, m_appConfig.ClientSecret, m_appConfig.AppName, m_appConfig.AppVersion,
            k_IsDataBufferUsing);
        
        m_emotivInterface.Start();

        foreach (string actionName in m_actionNames)
        {
            m_signatureActionsDict.Add(actionName, 0);
        }
    }

    private void OnEnable()
    {
        m_trainingUI.SubscribeToStartButton(() =>
        {
            if (m_emotivInterface.IsProfileLoaded)
            {
                if (m_trainingUI.currentActionIndex != -1)
                    m_emotivInterface.StartMCTraining(m_actionNames[m_trainingUI.currentActionIndex]);
            }
        });
        
        m_trainingUI.SubscribeToEraseButton(() =>
        {
            if (m_emotivInterface.IsProfileLoaded)
            {
                if (m_trainingUI.currentActionIndex != -1)
                    m_emotivInterface.EraseMCTraining(m_actionNames[m_trainingUI.currentActionIndex]);
            }
        });
        
        m_trainingUI.SubscribeToCancelButton(() =>
        {
            if (m_emotivInterface.IsProfileLoaded)
                m_emotivInterface.ResetMCTraining(m_actionNames[m_trainingUI.currentActionIndex]);
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
                m_emotivInterface.SubscribeData(new List<string> { "sys", "com" });
                m_emotivInterface.SysEventsReceived += OnSysEventsReceived;
                m_emotivInterface.MentalCommandReceived += OnMentalCommandReceived;

                m_emotivInterface.BciTraining.GetTrainedSignatureActionsOK += OnTrainedSignatureActions;
                m_emotivInterface.BciTraining.MentalCommandBrainMapOK += OnMentalCommandBrainMap;
                m_emotivInterface.BciTraining.MentalCommandTrainingThresholdOK += OnTrainingThresholdReceived;

                m_isSessionCreated = true;
            }

            if (!m_isProfileLoaded && m_emotivInterface.IsProfileLoaded)
            {
                m_emotivInterface.GetMCTrainedSignatureActions(k_ProfileName);
                m_emotivInterface.GetMCBrainMap(k_ProfileName);

                m_isProfileLoaded = true;
            }
        }

        ProcessEventQueues();
    }

    private void OnDisable()
    {
        m_trainingUI.UnsubscribeEventsFromAllButtons();
    }

    void OnApplicationQuit()
    {
        m_emotivInterface.Stop();
    }

    private void ProcessEventQueues()
    {
        if (m_sysEventQueue.Count > 0)
        {
            ProcessSystemEvent(m_sysEventQueue.Dequeue());
        }

        if (m_signatureActionsQueue.Count > 0)
        {
            ProcessTrainedSignatureActions(m_signatureActionsQueue.Dequeue());
        }

        if (m_brainMapQueue.Count > 0)
        {
            ProcessMentalCommandBrainMap(m_brainMapQueue.Dequeue());
        }

        if (m_trainingThresholdQueue.Count > 0)
        {
            ProcessCommandTrainingThreshold(m_trainingThresholdQueue.Dequeue());
        }
    }


    public void LoadProfile()
    {
        m_emotivInterface.LoadProfile(k_ProfileName);
    }
    
    private void OnSysEventsReceived(SysEventArgs data)
    {
        if (Enum.TryParse(data.EventMessage, out SystemEvent mentalEvent))
        {
            m_sysEventQueue.Enqueue(mentalEvent);
        }
        else
        {
            Debug.LogError("Invalid Event received from SysEvent stream.");
        }
    }
    
    private void OnMentalCommandReceived(MentalCommandEventArgs data)
    {
        if (data.Act == "pull")
        {
            m_playerTrainingWrapper.EnqueuePlayerMovementInput((float)data.Pow);
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
                m_emotivInterface.GetMCTrainingThreshold(k_ProfileName);
                break;

            case SystemEvent.MC_DataErased:
                m_emotivInterface.SaveProfile(k_ProfileName);
                break;

            case SystemEvent.MC_Reset:
                m_trainingUI.SetBaseState();
                break;

            case SystemEvent.MC_Completed:
                m_trainingUI.SetBaseState();
                m_playerTrainingWrapper.ResetPlayerPosition();

                m_emotivInterface.GetMCTrainedSignatureActions(k_ProfileName);
                m_emotivInterface.GetMCBrainMap(k_ProfileName);
                m_emotivInterface.SaveProfile(k_ProfileName);
                break;

            case SystemEvent.MC_Rejected:
                m_trainingUI.SetBaseState();
                m_playerTrainingWrapper.ResetPlayerPosition();
                break;
        }
    }

    private void OnTrainedSignatureActions(JObject result)
    {
        m_signatureActionsQueue.Enqueue(result);
    }
    
    private void OnMentalCommandBrainMap(JArray result)
    {
        m_brainMapQueue.Enqueue(result);
    }
    
    private void OnTrainingThresholdReceived(JObject result)
    {
        m_trainingThresholdQueue.Enqueue(result);
    }

    private void ProcessTrainedSignatureActions(JObject result)
    {
        JArray trainedActions = result["trainedActions"].Value<JArray>();

        for (int i = 0, l = trainedActions.Count; i < l; i++) 
        {
            string actionName = trainedActions[i]["action"].Value<string>();

            if (!m_checkedSignatureActionOnLoad || actionName == m_actionNames[m_trainingUI.currentActionIndex]) 
            {
                int skillLevel = trainedActions[i]["times"].Value<int>();

                m_signatureActionsDict[actionName] = skillLevel;

                m_trainingUI.UpdateSkillLevel(i, skillLevel);
            }
        }

        if (m_signatureActionsDict["neutral"] >= 1)
        {
            m_trainingUI.EnableAllActions();
        }

        if (!m_checkedSignatureActionOnLoad)
            m_checkedSignatureActionOnLoad = true;
    }

    private void ProcessMentalCommandBrainMap(JArray result)
    {
        if (result.Count > 0) 
        {
            foreach (JToken mapValue in result)
            {
                string actionName = mapValue["action"].Value<string>();

                int actionIndex = -1;
                for (int i = 0, l = m_actionNames.Length; i < l; i++)
                {
                    if (actionName == m_actionNames[i])
                    {
                        actionIndex = i;
                    }
                }

                JArray coordinatesArray = (JArray)mapValue["coordinates"];
                float[] coordinates = coordinatesArray.ToObject<float[]>();

                Vector2 coordinatesVector = new Vector2(coordinates[0], coordinates[1]);
                m_trainingUI.UpdateBrainMarkers(actionIndex, coordinatesVector);
            }
        }
        else if (m_signatureActionsDict.ContainsKey("neutral"))
        {
            Vector2 coordinatesVector = new Vector2(0f, 0f);
            m_trainingUI.UpdateBrainMarkers(0, coordinatesVector);
        }
    }

    private void ProcessCommandTrainingThreshold(JObject result)
    {
        if (m_trainingUI.currentActionIndex != 0)
        {
            float currentThreshold = result["currentThreshold"].Value<float>();
            float lastTrainingScore = result["lastTrainingScore"].Value<float>();

            string actionName = m_actionNames[m_trainingUI.currentActionIndex];
            if (m_signatureActionsDict.ContainsKey(actionName) && m_signatureActionsDict[actionName] > 1)
            {
                m_trainingUI.EnableTrainingThresholdElement(currentThreshold, lastTrainingScore);
            }
        }
    }
}