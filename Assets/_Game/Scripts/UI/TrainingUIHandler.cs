using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

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
        public string mActionName;

        public Transform mBrainmapMarker;

        public Image mButtonHighlightImage;

        public TextMeshProUGUI mSkillLevelText;

        public TrainingThresholdElement mThresholdElement;
    }

    [SerializeField] private float m_brainMapRadius = 200f;
    [SerializeField] private float m_thresholdIndicatorHalfRange = 70f;
    [SerializeField] private ActionUIElement[] m_actionUIElements;
    [SerializeField] private float m_fadeInDuration = 2f;
    [SerializeField] private Image m_fadeImage;
    [SerializeField] private Slider m_trainingTimeSlider;

    [Header("Training Buttons")]
    [SerializeField] private Button m_loadProfileButton;
    [SerializeField] private Button m_startButton;
    [SerializeField] private Button m_eraseButton;
    [SerializeField] private Button m_cancelButton;
    [SerializeField] private Button m_acceptButton;
    [SerializeField] private Button m_rejectButton;

    private int m_currentActionIndex = -1;

    private bool m_isTraining;
    
    public string CurrentSelectedAction => m_currentActionIndex != -1
        ? m_actionUIElements[m_currentActionIndex].mActionName : string.Empty;

    public bool IsTraining => m_isTraining;

    public void HandleProfileLoaded()
    {
        m_fadeImage.DOColor(new Color(m_fadeImage.color.r, m_fadeImage.color.g, m_fadeImage.color.b, 0f), m_fadeInDuration)
            .SetEase(Ease.InExpo).OnComplete(() => m_fadeImage.gameObject.SetActive(false));
        
        m_loadProfileButton.transform.parent.gameObject.SetActive(false);
    }

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
        m_startButton.transform.parent.gameObject.SetActive(true);
        m_eraseButton.transform.parent.gameObject.SetActive(true);

        m_cancelButton.transform.parent.gameObject.SetActive(false);

        m_acceptButton.transform.parent.gameObject.SetActive(false);
        m_rejectButton.transform.parent.gameObject.SetActive(false);

        DisableTrainingThresholdElement();

        m_isTraining = false;

        m_trainingTimeSlider.DOKill();
        m_trainingTimeSlider.value = 0f;
        m_trainingTimeSlider.gameObject.SetActive(false);
    }
    
    public void SetTrainingState()
    {
        m_startButton.transform.parent.gameObject.SetActive(false);
        m_eraseButton.transform.parent.gameObject.SetActive(false);

        m_cancelButton.transform.parent.gameObject.SetActive(true);

        m_trainingTimeSlider.DOValue(1f, EmotivTrainingInterfacer.k_TrainingDuration).SetEase(Ease.Linear);
        m_trainingTimeSlider.gameObject.SetActive(true);

        m_isTraining = true;
    }

    public void SetTrainedState()
    {
        m_cancelButton.transform.parent.gameObject.SetActive(false);

        m_acceptButton.transform.parent.gameObject.SetActive(true);
        m_rejectButton.transform.parent.gameObject.SetActive(true);
    }

    public void EnableActionOptions(bool enable = true)
    {
        for (int i = 1, l = m_actionUIElements.Length; i < l; i++)
        {
            ActionUIElement actionElement = m_actionUIElements[i];
            actionElement.mButtonHighlightImage.gameObject.SetActive(enable);
        }
    }

    public void UpdateSkillLevel(Dictionary<string, int> actionsDict)
    {
        foreach (ActionUIElement action in m_actionUIElements)
        {
            TextMeshProUGUI skillLevelText = action.mSkillLevelText;
            
            if (actionsDict.ContainsKey(action.mActionName))
            {
                skillLevelText.text = actionsDict[action.mActionName].ToString();
            }
            else
            {
                skillLevelText.text = "0";
            }
        }
    }
    
    public void UpdateBrainMarkers(string actionName, Vector2 coordinates)
    {
        foreach (ActionUIElement action in m_actionUIElements)
        {
            if (actionName == action.mActionName)
            {
                Transform brainmapMarker = action.mBrainmapMarker;

                if (coordinates != Vector2.zero || action.mActionName == "neutral")
                {
                    float radius = Mathf.Sqrt(coordinates.x * coordinates.x + coordinates.y * coordinates.y) *
                                   m_brainMapRadius;
                    float angle = Mathf.Atan2(coordinates.y, coordinates.x) * 2;

                    Vector2 newCoordinates = new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
                    brainmapMarker.localPosition = newCoordinates;

                    if (!brainmapMarker.gameObject.activeSelf)
                        brainmapMarker.gameObject.SetActive(true);
                }
                else
                {
                    brainmapMarker.gameObject.SetActive(false);
                }
            }
        }
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
