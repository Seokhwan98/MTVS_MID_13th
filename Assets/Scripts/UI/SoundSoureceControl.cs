using System;
using UnityEngine;

public class SoundSoureceControl : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource?.Play();
    }
}
