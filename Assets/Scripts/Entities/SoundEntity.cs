using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEntity : MonoBehaviour
{
    public bool WorldSound = true;

    public void PlaySound(string key)
    {
        if (WorldSound)
        {
            AudioControl.Instance.PlayInPosition(key, transform.position);
        }
        else
        {
            AudioControl.Instance.Play(key);
        }
    }

    public void PlayMusic(string key)
    {
        AudioControl.Instance.SetMusic(key);
    }
}
