using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicHandler : MonoBehaviour
{
    [SerializeField] private AudioClip[] m_bgmAudioClips;

    AudioSource m_audioSource;

    private int m_lastClipIndex = -1;

    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        PlayMusic();
    }

    private void Update()
    {
        if (!m_audioSource.isPlaying)
        {
            PlayMusic();
        }
    }

    private void PlayMusic()
    {
        int clipIndex = Random.Range(0, m_bgmAudioClips.Length);
        while (clipIndex == m_lastClipIndex)
        {
            clipIndex = Random.Range(0, m_bgmAudioClips.Length);
        }

        AudioClip clip = m_bgmAudioClips[clipIndex];

        m_audioSource.clip = clip;
        m_audioSource.Play();


        m_lastClipIndex = clipIndex;
    }
}
