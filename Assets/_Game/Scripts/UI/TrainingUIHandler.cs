using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class TrainingUIHandler : MonoBehaviour
{
    [System.Serializable]
    private struct TrainingThresholdElement
    {
        public Transform mThresholdIndicator;
        
        public Slider mThresholdSlider;
    }

    [System.Serializable]
    private struct ActionUIElement
    {
        public Transform mBrainmapMarker;

        public Image mButtonHighlightImage;

        public TextMeshProUGUI mSkillLevelText;

        public TrainingThresholdElement mThresholdElement;
    }

    [SerializeField] private float m_brainMapRadius = 200f;
    [SerializeField] private float m_thresholdIndicatorHalfRange = 70f;
    [SerializeField] private ActionUIElement[] m_actionUIElements;

    [Header("Training Buttons")]
    [SerializeField] private Button m_loadProfileButton;
    [SerializeField] private Button m_startButton;
    [SerializeField] private Button m_eraseButton;
    [SerializeField] private Button m_cancelButton;
    [SerializeField] private Button m_acceptButton;
    [SerializeField] private Button m_rejectButton;

    private int m_currentActionIndex = -1;

    private bool m_isTraining;

    public int currentActionIndex => m_currentActionIndex;

    public void SelectAction(int actionIndex)
    {
        if (!m_isTraining) 
        {
            foreach (ActionUIElement actionUI in m_actionUIElements)
            {
                actionUI.mButtonHighlightImage.enabled = false;
            }

            m_currentActionIndex = actionIndex;
            m_actionUIElements[m_currentActionIndex].mButtonHighlightImage.enabled = true;
        }
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

        DisableTrainingThresholdElement();

        m_isTraining = false;
    }
    
    public void SetTrainingState()
    {
        m_startButton.gameObject.SetActive(false);
        m_eraseButton.gameObject.SetActive(false);

        m_cancelButton.gameObject.SetActive(true);

        m_isTraining = true;
    }

    public void SetTrainedState()
    {
        m_cancelButton.gameObject.SetActive(false);

        m_acceptButton.gameObject.SetActive(true);
        m_rejectButton.gameObject.SetActive(true);
    }

    public void EnableAllActions()
    {
        for (int i = 1, l = m_actionUIElements.Length; i < l; i++)
        {
            ActionUIElement actionElement = m_actionUIElements[i];
            actionElement.mButtonHighlightImage.gameObject.SetActive(true);
        }
    }

    public void UpdateSkillLevel(int level)
    {
        TextMeshProUGUI skillLevelText = m_actionUIElements[m_currentActionIndex].mSkillLevelText;
        skillLevelText.text = level.ToString();
    }
    
    public void UpdateBrainMarkers(int index, Vector2 coordinates)
    {
        Transform brainmapMarker = m_actionUIElements[index].mBrainmapMarker;

        float radius = Mathf.Sqrt(coordinates.x * coordinates.x + coordinates.y * coordinates.y) * m_brainMapRadius;
        float angle = Mathf.Atan2(coordinates.y, coordinates.x);

        float newRadius = 2 * angle;

        Vector2 newCoordinates = new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
        brainmapMarker.localPosition = newCoordinates;

        if (!brainmapMarker.gameObject.activeSelf)
            brainmapMarker.gameObject.SetActive(true);
    }

    public void EnableTrainingThresholdElement(float currentThreshold, float lastTrainingScore)
    {
        TrainingThresholdElement thresholdElement = m_actionUIElements[m_currentActionIndex].mThresholdElement;

        Vector3 indicatorPosition = thresholdElement.mThresholdIndicator.localPosition;
        indicatorPosition.x = Mathf.Lerp(-m_thresholdIndicatorHalfRange, m_thresholdIndicatorHalfRange, currentThreshold);
        thresholdElement.mThresholdIndicator.localPosition = indicatorPosition;

        thresholdElement.mThresholdSlider.value = lastTrainingScore;

        thresholdElement.mThresholdIndicator.parent.gameObject.SetActive(true);
    }
    
    private void DisableTrainingThresholdElement()
    {
        TrainingThresholdElement thresholdElement = m_actionUIElements[m_currentActionIndex].mThresholdElement;
        thresholdElement.mThresholdIndicator.parent.gameObject.SetActive(false);
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
