using UnityEngine;

public class VoiceStart : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    private void Start()
    {
        audioSource?.Play();
        
    }
}