using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEntity : MonoBehaviour
{

    public void PlaySound(string key)
    {
        AudioControl.Instance.Play(key);
    }

    public void PlayMusic(string key)
    {
        AudioControl.Instance.SetMusic(key);
    }
}
