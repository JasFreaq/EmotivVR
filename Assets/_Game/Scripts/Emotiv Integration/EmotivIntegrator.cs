using EmotivUnityPlugin;
using EmotivVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using static UnityEngine.Rendering.DebugUI;

public class EmotivIntegrator : MonoBehaviour
{
    [SerializeField] private ApplicationConfiguration m_appConfig;

    private EmotivUnityItf m_emotivInterface = EmotivUnityItf.Instance;

    private const float k_TimeUpdateData = 1f;

    private const bool k_IsDataBufferUsing = false; // default subscribed data will not saved to Data buffer

    private const string k_ProfileName = "Default"; // default profile name for playtesting

    private float m_timerDataUpdate = 0;
    
    private void Start()
    {
        // init EmotivUnityItf without data buffer using
        m_emotivInterface.Init(m_appConfig.ClientId, m_appConfig.ClientSecret, m_appConfig.AppName, m_appConfig.AppVersion,
            k_IsDataBufferUsing);

        // Start
        m_emotivInterface.Start();
    }

    private void Update()
    {
        m_timerDataUpdate += Time.deltaTime;
        if (m_timerDataUpdate < k_TimeUpdateData)
            return;

        m_timerDataUpdate -= k_TimeUpdateData;
        
        if (!m_emotivInterface.IsAuthorizedOK)
            return;
        

    }
}
