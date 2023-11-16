using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace EmotivUnityPlugin
{
    /// <summary>
    /// Reponsible for training.
    /// </summary>
    public class TrainingHandler
    {

        static readonly object _locker = new object();
        private CortexClient _ctxClient = CortexClient.Instance;

        private Authorizer _authorizer = Authorizer.Instance;
        private SessionHandler _sessionHandler = SessionHandler.Instance;


        // event
        public event EventHandler<List<string>> QueryProfileOK;

        public event EventHandler<string> ProfileLoaded;

        public event EventHandler<bool> ProfileUnLoaded;

        public event EventHandler<bool> TrainingSucceeded;

        public event EventHandler<bool> ReadyForTraining;

        public event EventHandler<string> ProfileSavedOK
        {
            add { _ctxClient.SaveProfileOK += value; }
            remove { _ctxClient.SaveProfileOK -= value; }
        }

        public event EventHandler<JObject> TrainingOK
        {
            add { _ctxClient.TrainingOK += value; }
            remove { _ctxClient.TrainingOK -= value; }
        }

        public event EventHandler<string> CreateProfileOK
        {
            add { _ctxClient.CreateProfileOK += value; }
            remove { _ctxClient.CreateProfileOK -= value; }
        }

        public event EventHandler<JObject> GetCurrentProfileDone
        {
            add { _ctxClient.GetCurrentProfileDone += value; }
            remove { _ctxClient.GetCurrentProfileDone -= value; }
        }

        public event EventHandler<JObject> GetTrainedSignatureActionsOK
        {
            add { _ctxClient.GetTrainedSignatureActionsOK += value; }
            remove { _ctxClient.GetTrainedSignatureActionsOK -= value; }
        }
        
        public event EventHandler<JObject> GetMentalCommandActiveActionOK
        {
            add { _ctxClient.MentalCommandActiveActionOK += value; }
            remove { _ctxClient.MentalCommandActiveActionOK -= value; }
        }
        
        public event EventHandler<JArray> GetMentalCommandBrainMapOK
        {
            add { _ctxClient.MentalCommandBrainMapOK += value; }
            remove { _ctxClient.MentalCommandBrainMapOK -= value; }
        }
        
        public event EventHandler<JObject> GetMentalCommandTrainingThresholdOK
        {
            add { _ctxClient.MentalCommandTrainingThresholdOK += value; }
            remove { _ctxClient.MentalCommandTrainingThresholdOK -= value; }
        }

        public event EventHandler<DetectionInfo> GetDetectionInfoOK;

        public static TrainingHandler Instance { get; } = new TrainingHandler();

        //Constructor
        public TrainingHandler()
        {
            // Event register
            _ctxClient.GetDetectionInfoDone += OnGetDetectionOk;
            _ctxClient.LoadProfileOK += OnProfileLoadedOK;
            _ctxClient.UnloadProfileDone += OnUnloadProfileDone;
            _ctxClient.QueryProfileOK += OnQueryProfileOK;
        }

        private void OnUnloadProfileDone(object sender, bool isSuccess)
        {
            ProfileUnLoaded(this, isSuccess);
        }

        private void OnQueryProfileOK(object sender, JArray profiles)
        {
            UnityEngine.Debug.Log("QueryProfileOK" + profiles);
            List<string> profileLists = new List<string>();
            foreach (JObject ele in profiles)
            {
                string name = (string)ele["name"];
                profileLists.Add(name);
            }
            QueryProfileOK(this, profileLists);
        }

        private void OnProfileLoadedOK(object sender, string profileName)
        {
            ProfileLoaded(this, profileName);
        }

        private void OnGetDetectionOk(object sender, JObject data)
        {
            UnityEngine.Debug.Log("GetDetectionInfoOK: " + data);
            DetectionInfo detectioninfo = new DetectionInfo("mentalCommand");

            JArray actions = (JArray)data["actions"];
            foreach (var ele in actions)
            {
                detectioninfo.Actions.Add(ele.ToString());
            }
            JArray controls = (JArray)data["controls"];
            foreach (var ele in actions)
            {
                detectioninfo.Controls.Add(ele.ToString());
            }
            JArray events = (JArray)data["events"];
            foreach (var ele in actions)
            {
                detectioninfo.Events.Add(ele.ToString());
            }
            JArray signature = (JArray)data["signature"];
            foreach (var ele in actions)
            {
                detectioninfo.Signature.Add(ele.ToString());
            }
            GetDetectionInfoOK(this, detectioninfo);
        }

        /// <summary>
        /// Clear
        /// </summary>
        public void Clear()
        {

        }

        public void QueryProfile()
        {
            // query profiles
            string cortexToken = _authorizer.CortexToken;
            _ctxClient.QueryProfile(cortexToken);
        }

        /// <summary>
        /// Get useful detection information.
        /// </summary>
        public void GetDetectionInfo(string detection)
        {
            string cortexToken = _authorizer.CortexToken;
            _ctxClient.GetDetectionInfo(detection);
        }

        /// <summary>
        /// Get the training profile that is currently loaded for a specific headset
        /// </summary>
        public void GetCurrentProfile(string headsetId)
        {
            _ctxClient.GetCurrentProfile(_authorizer.CortexToken, headsetId);
        }

        public void GetTrainedSignatureActions(string detection, string profileName)
        {
            _ctxClient.GetTrainedSignatureActions(_authorizer.CortexToken, detection, profileName,
                _sessionHandler.SessionId);
        }

        public void MentalCommandActiveAction(string status, string profileName, string[] actions)
        {
            _ctxClient.MentalCommandActiveAction(_authorizer.CortexToken, status, profileName,
                _sessionHandler.SessionId, actions);
        }
        
        public void MentalCommandBrainMap(string profileName)
        {
            _ctxClient.MentalCommandBrainMap(_authorizer.CortexToken, profileName,
                _sessionHandler.SessionId);
        }
        
        public void MentalCommandTrainingThreshold(string profileName)
        {
            _ctxClient.MentalCommandTrainingThreshold(_authorizer.CortexToken, profileName,
                _sessionHandler.SessionId);
        }

        public void DoTraining(string action, string status, string detection)
        {
            UnityEngine.Debug.Log(status + " " + action + " training.");
            //Do training
            string cortexToken = _authorizer.CortexToken;
            string sessionId = _sessionHandler.SessionId;
            _ctxClient.Training(cortexToken, sessionId, status, detection, action);
        }

        public void CreateProfile(string profileName, string headsetId)
        {
            string cortexToken = _authorizer.CortexToken;
            _ctxClient.SetupProfile(cortexToken, profileName, "create", headsetId);
        }

        public void LoadProfile(string profileName, string headsetId)
        {

            string cortexToken = _authorizer.CortexToken;
            _ctxClient.SetupProfile(cortexToken, profileName, "load", headsetId);
        }

        public void UnLoadProfile(string profileName, string headsetId)
        {
            string cortexToken = _authorizer.CortexToken;
            _ctxClient.SetupProfile(cortexToken, profileName, "unload", headsetId);
        }

        public void SaveProfile(string profileName, string headsetId)
        {
            string cortexToken = _authorizer.CortexToken;
            _ctxClient.SetupProfile(cortexToken, profileName, "save", headsetId);
        }
    }
}
