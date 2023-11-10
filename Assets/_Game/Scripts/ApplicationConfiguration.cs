using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmotivVR
{
    [CreateAssetMenu(fileName = "New App Config", menuName = "App Config", order = 0)]
    public class ApplicationConfiguration : ScriptableObject
    {
        [SerializeField] private string m_appUrl = "wss://localhost:6868";

        [SerializeField] private string m_appName = "UnityApp";

        [SerializeField] private string m_clientId;

        [SerializeField] private string m_clientSecret;

        [SerializeField] private string m_appVersion = "3.3.0";

        public string AppUrl => m_appUrl;

        public string AppName => m_appName;

        public string ClientId => m_clientId;

        public string ClientSecret => m_clientSecret;

        public string AppVersion => m_appVersion;
    }

}