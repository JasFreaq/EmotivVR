using System;
using System.Collections;
using EmotivUnityPlugin;
using EmotivVR;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Unity.Entities.UniversalDelegates;

public class EmotivTrainingInterfacer : MonoBehaviour
{
    public const float k_TrainingDuration = 8f;

    [SerializeField] private ApplicationConfiguration m_appConfig;

    [SerializeField] private TrainingUIHandler m_trainingUI;
    
    private EmotivUnityItf m_emotivInterface = EmotivUnityItf.Instance;

    private PlayerControlWrapper m_playerControlWrapper;

    private const float k_timeUpdateData = 1f;

    private const bool k_isDataBufferUsing = false; // default subscribed data will not saved to Data buffer
    
    private Queue<SystemEvent> m_sysEventQueue = new Queue<SystemEvent>();

    private Queue<JObject> m_signatureActionsQueue = new Queue<JObject>();
    private Queue<JArray> m_brainMapQueue = new Queue<JArray>();
    private Queue<JObject> m_trainingThresholdQueue = new Queue<JObject>();

    private Dictionary<string, int> m_signatureActionsDict = new Dictionary<string, int>();

    private float m_timerDataUpdate;
    
    private bool m_isSessionCreated;
    
    private bool m_isProfileLoaded;
    
    private void Awake()
    {
        m_playerControlWrapper = GetComponent<PlayerControlWrapper>();
    }

    private void Start()
    {
        m_emotivInterface.Init(m_appConfig.ClientId, m_appConfig.ClientSecret, m_appConfig.AppName, m_appConfig.AppVersion,
            k_isDataBufferUsing);
        
        m_emotivInterface.Start();
    }

    private void OnEnable()
    {
        m_trainingUI.SubscribeToStartButton(() =>
        {
            if (m_emotivInterface.IsProfileLoaded)
            {
                if (m_trainingUI.CurrentSelectedAction != string.Empty)
                    m_emotivInterface.StartMCTraining(m_trainingUI.CurrentSelectedAction);
            }
        });
        
        m_trainingUI.SubscribeToEraseButton(() =>
        {
            if (m_emotivInterface.IsProfileLoaded)
            {
                if (m_trainingUI.CurrentSelectedAction != string.Empty)
                    m_emotivInterface.EraseMCTraining(m_trainingUI.CurrentSelectedAction);
            }
        });
        
        m_trainingUI.SubscribeToCancelButton(() =>
        {
            if (m_emotivInterface.IsProfileLoaded)
                m_emotivInterface.ResetMCTraining(m_trainingUI.CurrentSelectedAction);
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
        
        if (m_timerDataUpdate - k_timeUpdateData >= Mathf.Epsilon)
        {
            m_timerDataUpdate -= k_timeUpdateData;
            
            if (!m_isSessionCreated && m_emotivInterface.IsSessionCreated)
            {
                m_emotivInterface.SubscribeData(new List<string> { "sys", "com"});
                m_emotivInterface.SysEventsReceived += OnSysEventsReceived;
                m_emotivInterface.MentalCommandReceived += OnMentalCommandReceived;

                m_emotivInterface.BciTraining.GetTrainedSignatureActionsOK += OnTrainedSignatureActions;
                m_emotivInterface.BciTraining.MentalCommandBrainMapOK += OnMentalCommandBrainMap;
                m_emotivInterface.BciTraining.MentalCommandTrainingThresholdOK += OnTrainingThresholdReceived;

                m_trainingUI.EnableLoadProfileButton();

                m_isSessionCreated = true;
            }

            if (m_emotivInterface.IsProfileLoaded)
            {
                if (!m_isProfileLoaded) 
                {
                    m_trainingUI.HandleProfileLoaded();
                    m_playerControlWrapper.HandleProfileLoaded();

                    m_isProfileLoaded = true;
                }

                m_emotivInterface.GetMCTrainedSignatureActions(EmotivGameplayInterfacer.ProfileName);
                m_emotivInterface.GetMCBrainMap(EmotivGameplayInterfacer.ProfileName);
            }
        }

        ProcessEventQueues();
    }

    private void OnDisable()
    {
        m_trainingUI.UnsubscribeEventsFromAllButtons();
    }

    private void OnDestroy()
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

    public void CreateSession()
    {
        m_emotivInterface.CreateSessionWithHeadset("");
    }
    
    public void LoadProfile()
    {
        m_emotivInterface.LoadProfile(EmotivGameplayInterfacer.ProfileName);
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
        if (m_trainingUI.IsTraining)
        {
            if (data.Act == m_trainingUI.CurrentSelectedAction)
            {
                if (data.Act == "pull")
                {
                    m_playerControlWrapper.EnqueuePlayerMovementInput((float)data.Pow);
                }

                m_playerControlWrapper.IsPlayerLaserInput = data.Act == "push";
            }
        }
        else
        {
            m_playerControlWrapper.IsPlayerLaserInput = false;
        }
    }

    private void ProcessSystemEvent(SystemEvent sysEvent)
    {
        switch (sysEvent)
        {
            case SystemEvent.MC_Started:
                m_trainingUI.SetTrainingState();
                break;

            case SystemEvent.MC_Succeeded:
                m_trainingUI.SetTrainedState();
                m_emotivInterface.GetMCTrainingThreshold(EmotivGameplayInterfacer.ProfileName);
                break;

            case SystemEvent.MC_DataErased:
                m_signatureActionsDict[m_trainingUI.CurrentSelectedAction] = 0;

                m_emotivInterface.GetMCTrainedSignatureActions(EmotivGameplayInterfacer.ProfileName);
                m_emotivInterface.GetMCBrainMap(EmotivGameplayInterfacer.ProfileName);

                m_emotivInterface.SaveProfile(EmotivGameplayInterfacer.ProfileName);
                break;

            case SystemEvent.MC_Reset:
                m_trainingUI.SetBaseState();
                break;

            case SystemEvent.MC_Completed:
                m_trainingUI.SetBaseState();

                m_emotivInterface.GetMCTrainedSignatureActions(EmotivGameplayInterfacer.ProfileName);
                m_emotivInterface.GetMCBrainMap(EmotivGameplayInterfacer.ProfileName);

                m_emotivInterface.SaveProfile(EmotivGameplayInterfacer.ProfileName);

                if (m_trainingUI.CurrentSelectedAction == "pull") 
                    m_playerControlWrapper.ResetPlayerPosition();
                break;

            case SystemEvent.MC_Rejected:
                m_trainingUI.SetBaseState();

                if (m_trainingUI.CurrentSelectedAction == "pull")
                    m_playerControlWrapper.ResetPlayerPosition();
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
        
        foreach (JToken trainedAction in trainedActions)
        {
            string actionName = trainedAction["action"].Value<string>();
            int skillLevel = trainedAction["times"].Value<int>();

            m_signatureActionsDict[actionName] = skillLevel;
        }

        m_trainingUI.UpdateSkillLevel(m_signatureActionsDict);

        if (m_signatureActionsDict.ContainsKey("neutral") && m_signatureActionsDict["neutral"] >= 1)
        {
            m_trainingUI.EnableActionOptions();
        }
        else 
        {
            m_trainingUI.EnableActionOptions(false);
        }
    }

    private void ProcessMentalCommandBrainMap(JArray result)
    {
        if (result.Count > 0) 
        {
            foreach (JToken mapValue in result)
            {
                string actionName = mapValue["action"].Value<string>();
                
                JArray coordinatesArray = (JArray)mapValue["coordinates"];
                float[] coordinates = coordinatesArray.ToObject<float[]>();

                Vector2 coordinatesVector = new Vector2(coordinates[0], coordinates[1]);
                m_trainingUI.UpdateBrainMarkers(actionName, coordinatesVector);
            }
        }
        else if (m_signatureActionsDict.ContainsKey("neutral"))
        {
            Vector2 coordinatesVector = new Vector2(0f, 0f);
            m_trainingUI.UpdateBrainMarkers("neutral", coordinatesVector);
        }
    }

    private void ProcessCommandTrainingThreshold(JObject result)
    {
        if (m_trainingUI.CurrentSelectedAction != "neutral")
        {
            float currentThreshold = result["currentThreshold"].Value<float>();
            float lastTrainingScore = result["lastTrainingScore"].Value<float>();

            string actionName = m_trainingUI.CurrentSelectedAction;
            if (m_signatureActionsDict.ContainsKey(actionName) && m_signatureActionsDict[actionName] > 1)
            {
                m_trainingUI.EnableTrainingThresholdElement(currentThreshold, lastTrainingScore);
            }
        }
    }
}