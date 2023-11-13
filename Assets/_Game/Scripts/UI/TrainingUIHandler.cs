using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TrainingUIHandler : MonoBehaviour
{
    [System.Serializable]
    private struct ProfileData
    {
        public Transform mBrainmapMarker;

        public Image mButtonHighlightImage;

        public string mProfileName;
    }

    [SerializeField] private ProfileData[] m_profileData;

    [Header("Buttons")]
    [SerializeField] private Button m_loadProfileButton;
    [SerializeField] private Button m_startButton;
    [SerializeField] private Button m_eraseButton;
    [SerializeField] private Button m_cancelButton;
    [SerializeField] private Button m_acceptButton;
    [SerializeField] private Button m_rejectButton;

    private ProfileData m_currentProfile;

    public string CurrentProfileName => m_currentProfile.mProfileName;
    
    public void SelectProfile(int profileIndex)
    {
        foreach (ProfileData profile in m_profileData)
        {
            profile.mButtonHighlightImage.enabled = false;
        }

        m_currentProfile = m_profileData[profileIndex];
        m_currentProfile.mButtonHighlightImage.enabled = true;
    }

    public void EnableLoadProfileButton()
    {
        if (!m_loadProfileButton.interactable)
            m_loadProfileButton.interactable = true;
    }

    public void SetBaseState()
    {
        m_startButton.gameObject.SetActive(true);
        m_eraseButton.gameObject.SetActive(true);
        
        m_acceptButton.gameObject.SetActive(false);
        m_rejectButton.gameObject.SetActive(false);
    }
    
    public void SetTrainingState()
    {
        m_startButton.gameObject.SetActive(false);
        m_eraseButton.gameObject.SetActive(false);
        Debug.Log("Test");

        m_cancelButton.gameObject.SetActive(true);
    }

    public void SetTrainedState()
    {
        m_cancelButton.gameObject.SetActive(false);

        m_acceptButton.gameObject.SetActive(true);
        m_rejectButton.gameObject.SetActive(true);
    }

    public void SubscribeToStartButton(Action action)
    {
        m_startButton.onClick.AddListener(new UnityAction(action));
    }

    public void SubscribeToEraseButton(Action action)
    {
        m_eraseButton.onClick.AddListener(new UnityAction(action));
    }

    public void SubscribeToCancelButton(Action action)
    {
        m_cancelButton.onClick.AddListener(new UnityAction(action));
    }

    public void SubscribeToAcceptButton(Action action)
    {
        m_acceptButton.onClick.AddListener(new UnityAction(action));
    }

    public void SubscribeToRejectButton(Action action)
    {
        m_rejectButton.onClick.AddListener(new UnityAction(action));
    }

    public void UnsubscribeEventsFromAllButtons()
    {
        m_startButton.onClick.RemoveAllListeners();
        m_eraseButton.onClick.RemoveAllListeners();
        m_cancelButton.onClick.RemoveAllListeners();
        m_acceptButton.onClick.RemoveAllListeners();
        m_rejectButton.onClick.RemoveAllListeners();
    }
}
