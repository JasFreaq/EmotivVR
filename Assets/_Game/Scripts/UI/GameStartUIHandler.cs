using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartUIHandler : MonoBehaviour
{
    [SerializeField] private float m_fadeInDuration = 2f;
    [SerializeField] private Image m_fadeImage;

    [SerializeField] private Button m_createSessionButton;
    [SerializeField] private Button m_loadProfileButton;

    public void HandleProfileLoaded()
    {
        m_fadeImage.DOColor(new Color(m_fadeImage.color.r, m_fadeImage.color.g, m_fadeImage.color.b, 0f), m_fadeInDuration)
            .SetEase(Ease.InExpo).OnComplete(() => gameObject.SetActive(false));
    }

    public void EnableCreateSessionButton()
    {
        if (!m_createSessionButton.interactable)
            m_createSessionButton.interactable = true;
    }
    
    public void EnableLoadProfileButton()
    {
        if (!m_loadProfileButton.interactable)
            m_loadProfileButton.interactable = true;
    }
}
