using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileSelectionUIHandler : MonoBehaviour
{
    private const string k_ProfilePrefix = "LaserKnight";

    [SerializeField] private TMP_Dropdown m_profileDropdown;

    private void Start()
    {
        m_profileDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void OnDropdownValueChanged(int index)
    {
        string selectedProfile = $"{k_ProfilePrefix} {m_profileDropdown.options[index].text}";

        EmotivGameplayInterfacer.ProfileName = selectedProfile;
    }
}
