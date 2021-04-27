using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public List<AudioClip> m_audioClips = new List<AudioClip>();
    private static AudioSource m_audioSource;

    private void Awake() {
        m_audioSource = GetComponent<AudioSource>();
        m_audioSource.clip = m_audioClips[0];
    }

    private void Update() {
        if(!m_audioSource.isPlaying){
            m_audioSource.Play();
        }
    }
    public static void Play(float _distance){
        m_audioSource.volume = (1 / (_distance * _distance) * 100);
    }
}
