using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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

    public void SetPlaylist(int playlistIndex)
    {
        AudioControl.Instance.SetPlaylist(playlistIndex);
    }

    public void SetMusic(int musicIndex)
    {
        AudioControl.Instance.SetPlaylistIndex(musicIndex);
    }

}
