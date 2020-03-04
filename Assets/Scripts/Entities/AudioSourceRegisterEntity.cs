using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceRegisterEntity : MonoBehaviour
{
    public AudioSource audioSource;
    

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        AudioControl.Instance.RegisterAudiosource(audioSource);
        Destroy(this);
    }
}
