using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDHandler : MonoBehaviour
{
    [SerializeField] private Slider m_healthSlider;
    [SerializeField] private TextMeshProUGUI m_scoreText;

    public void UpdateHealth(float ratio)
    {
        m_healthSlider.DOValue(ratio, 0.2f);
    }

    public void UpdateScore(int score)
    {
        m_scoreText.text = score.ToString();
    }
}
