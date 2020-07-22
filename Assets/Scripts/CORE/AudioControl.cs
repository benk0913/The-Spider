using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(ResourcesLoader))]
public class AudioControl : MonoBehaviour {

    #region Essential
    public  string m_sInstancePrefab;

    public List<GameObject> Instances   = new List<GameObject>();
    public Dictionary<string, float> VolumeGroups = new Dictionary<string, float>();

    public static AudioControl Instance;

    [SerializeField]
    protected Transform m_tInstancesContainer;

    [SerializeField]
    protected AudioSource MusicSource;

    public List<MusicPlaylist> MusicPlayLists = new List<MusicPlaylist>();

    public MusicPlaylist CurrentPlaylist = new MusicPlaylist();

    void Awake()
    {
        Instance = this;

        VolumeGroups.Add("Untagged", PlayerPrefs.GetFloat("Untagged", 1f));
        VolumeGroups.Add("Music", PlayerPrefs.GetFloat("Music", 0.6f));

        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1f));

        if (MusicSource != null)
        {
            MusicSource.volume = VolumeGroups["Music"];
        }

    }

    #endregion

    #region Methods

    public void PlayInPosition(string gClip, Vector3 pos, float MaxDistance = 47f, bool gLoop = false)
    {
        if(CORE.Instance.isLoading)
        {
            return;
        }

        GameObject currentInstance = null;

        for (int i = 0; i < Instances.Count; i++)
        {
            if(Instances[i] == null || Instances[i].GetComponent<AudioSource>() == null)
            {
                Instances.RemoveAt(i);
                continue;
            }

            if (!Instances[i].GetComponent<AudioSource>().isPlaying)
            {
                currentInstance = Instances[i];
                break;
            }
        }

        if (currentInstance == null)
        {
            currentInstance = (GameObject)Instantiate(ResourcesLoader.Instance.GetObject(m_sInstancePrefab));
            currentInstance.transform.parent = m_tInstancesContainer;
            Instances.Add(currentInstance);
        }

        currentInstance.transform.position = pos;
        
        AudioSource source = currentInstance.GetComponent<AudioSource>();
        source.spatialBlend = 1f;
        source.maxDistance = MaxDistance;
        source.pitch = 1f;
        source.loop = gLoop;
        source.clip = ResourcesLoader.Instance.GetClip(gClip);
        source.Play();

        if (VolumeGroups.ContainsKey(currentInstance.tag))
        {
            currentInstance.GetComponent<AudioSource>().volume = VolumeGroups[currentInstance.tag];
        }
        else
        {
        }
    }

    public void Play(string gClip)
    {
        if (CORE.Instance.isLoading)
        {
            return;
        }

        GameObject currentInstance = null;

        for (int i=0;i<Instances.Count;i++)
        {
            if(!Instances[i].GetComponent<AudioSource>().isPlaying)
            {
                currentInstance = Instances[i];
                break;
            }
        }

        if(currentInstance==null)
        {
            GameObject prefab = ResourcesLoader.Instance.GetObject(m_sInstancePrefab);

            if(prefab == null)
            {
                return;
            }

            currentInstance = (GameObject)Instantiate(prefab);
            currentInstance.transform.parent = m_tInstancesContainer;
            Instances.Add(currentInstance);
        }

        AudioSource source = currentInstance.GetComponent<AudioSource>();
        source.spatialBlend = 0f;
        source.pitch = 1f;
        source.loop = false;
        source.clip = ResourcesLoader.Instance.GetClip(gClip);
        source.Play();

        if (VolumeGroups.ContainsKey(currentInstance.tag))
        {
            source.volume = VolumeGroups[currentInstance.tag];
        }
    }

    public void Play(string gClip, bool gLoop)
    {
        if (CORE.Instance.isLoading)
        {
            return;
        }

        GameObject currentInstance = null;

        for (int i = 0; i < Instances.Count; i++)
        {
            if (!Instances[i].GetComponent<AudioSource>().isPlaying)
            {
                currentInstance = Instances[i];
                break;
            }
        }

        if (currentInstance == null)
        {
            currentInstance = (GameObject)Instantiate(ResourcesLoader.Instance.GetObject(m_sInstancePrefab));
            currentInstance.transform.parent = m_tInstancesContainer;
            Instances.Add(currentInstance);
        }

        AudioSource source = currentInstance.GetComponent<AudioSource>();
        source.spatialBlend = 0f;
        source.pitch = 1f;
        source.loop = gLoop;
        source.clip = ResourcesLoader.Instance.GetClip(gClip);
        source.Play();

        if (VolumeGroups.ContainsKey(currentInstance.tag))
        {
            source.volume = VolumeGroups[currentInstance.tag];
        }

    }

    public void Play(string gClip, bool gLoop, string gTag)
    {
        if (CORE.Instance.isLoading)
        {
            return;
        }

        GameObject currentInstance = null;

        for (int i = 0; i < Instances.Count; i++)
        {
            if (!Instances[i].GetComponent<AudioSource>().isPlaying)
            {
                currentInstance = Instances[i];
                break;
            }
        }

        if (currentInstance == null)
        {
            currentInstance = (GameObject)Instantiate(ResourcesLoader.Instance.GetObject(m_sInstancePrefab));
            currentInstance.transform.parent = m_tInstancesContainer;
            Instances.Add(currentInstance);
        }

        AudioSource source = currentInstance.GetComponent<AudioSource>();
        source.spatialBlend = 0f;
        source.pitch = 1f;
        source.loop = gLoop;
        source.clip = ResourcesLoader.Instance.GetClip(gClip);
        source.Play();

        currentInstance.tag = gTag;

        if(VolumeGroups.ContainsKey(currentInstance.tag))
        {
            source.volume = VolumeGroups[currentInstance.tag];
        }
    }

    public void SetVolume(string gTag, float gVolume)
    {

        PlayerPrefs.SetFloat(gTag, gVolume);
        PlayerPrefs.Save();

        if (gTag == "Music")
        {
            MusicSource.volume = gVolume;
        }
        
        if(!VolumeGroups.ContainsKey(gTag))
        {
            VolumeGroups.Add(gTag, gVolume);
        }
        else
        {
            VolumeGroups[gTag] = gVolume;
        }

        for (int i = 0; i < Instances.Count; i++)
        {
            if (Instances[i].tag == gTag)
            {
                Instances[i].GetComponent<AudioSource>().volume = gVolume;
            }
        }
    }

    public void SetTemporaryVolume(string gTag, float gVolume)
    {

        if (gTag == "Music")
        {
            MusicSource.volume = gVolume;
        }

        if (!VolumeGroups.ContainsKey(gTag))
        {
            VolumeGroups.Add(gTag, gVolume);
        }
        else
        {
            VolumeGroups[gTag] = gVolume;
        }

        for (int i = 0; i < Instances.Count; i++)
        {
            if(Instances[i] == null)
            {
                Instances.RemoveAt(i);
                continue;
            }
            if (Instances[i].tag == gTag)
            {
                Instances[i].GetComponent<AudioSource>().volume = gVolume;
            }
        }
    }

    public void ResetTemporaryVolume(string gTag)
    {
        SetTemporaryVolume(gTag,PlayerPrefs.GetFloat(gTag,0.6f));
    }


    public void SetMasterVolume(float gVolume)
    {
        AudioListener.volume = gVolume;
        PlayerPrefs.SetFloat("MasterVolume", gVolume);
        PlayerPrefs.Save();
    }

    public void PlayWithPitch(string gClip,float fPitch)
    {
        if (CORE.Instance.isLoading)
        {
            return;
        }

        GameObject currentInstance = null;

        for (int i = 0; i < Instances.Count; i++)
        {
            if (!Instances[i].GetComponent<AudioSource>().isPlaying)
            {
                currentInstance = Instances[i];
                break;
            }
        }

        if (currentInstance == null)
        {
            currentInstance = (GameObject)Instantiate(ResourcesLoader.Instance.GetObject(m_sInstancePrefab));
            currentInstance.transform.parent = m_tInstancesContainer;
            Instances.Add(currentInstance);
        }

        currentInstance.GetComponent<AudioSource>().spatialBlend = 0f;
        currentInstance.GetComponent<AudioSource>().clip = ResourcesLoader.Instance.GetClip(gClip);
        currentInstance.GetComponent<AudioSource>().pitch = fPitch;
        currentInstance.GetComponent<AudioSource>().loop = false;
        currentInstance.GetComponent<AudioSource>().Play();

        if (VolumeGroups.ContainsKey(currentInstance.tag))
        {
            currentInstance.GetComponent<AudioSource>().volume = VolumeGroups[currentInstance.tag];
        }
    }

    public void SetPlaylist(int playlistIndex)
    {
        CurrentPlaylist = MusicPlayLists[playlistIndex];
        SetMusic(CurrentPlaylist.Playlist[Random.Range(0, CurrentPlaylist.Playlist.Count)]);
    }

    public void SetPlaylistIndex(int index)
    {
        if(CurrentPlaylist == null)
        {
            Debug.LogError("AUDIO - NO ACTIVE PLAYLIST");
            return;
        }

        if(CurrentPlaylist.Playlist.Count <= index)
        {
            Debug.LogError("AUDIO - BAD INDEX OF PLAYLIST");
            return;
        }

        SetMusic(CurrentPlaylist.Playlist[index]);
    }

    protected void SetMusic(string gClip, float fPitch = 1f)
    {

        if(WaitForMusicEndRoutineInstance != null)
        {
            StopCoroutine(WaitForMusicEndRoutineInstance);
        }

        MusicSource.volume = VolumeGroups["Music"];

        MusicSource.loop = false;

        if (string.IsNullOrEmpty(gClip))
        {
            MusicSource.Stop();
            MusicSource.clip = null;
            return;
        }

        MusicSource.pitch = fPitch;

        MusicSource.Stop();
        MusicSource.clip = ResourcesLoader.Instance.GetClip(gClip);
        MusicSource.Play();    

        WaitForMusicEndRoutineInstance = StartCoroutine(WaitForMusicEndRoutine());
    }

    Coroutine WaitForMusicEndRoutineInstance;

    IEnumerator WaitForMusicEndRoutine()
    { 
        yield return 0;

        while(MusicSource.isPlaying)
        {
            yield return 0;
        }

        WaitForMusicEndRoutineInstance = null;

        if (CurrentPlaylist != null)
        {
            int targetIndex = CurrentPlaylist.Playlist.IndexOf(MusicSource.clip.name) + 1;
            if(targetIndex >= CurrentPlaylist.Playlist.Count)
            {
                targetIndex = 0;
            }

            SetMusic(CurrentPlaylist.Playlist[targetIndex]);
        }        
    }

    public void StopSound(string gClip)
    {

        foreach(GameObject obj in Instances)
        {
            if(obj == null)
            {
                Instances.RemoveAll(x=>x == null);
                StopSound(gClip);
                return;
            }

            AudioSource source = obj.GetComponent<AudioSource>();
            
            if (source.isPlaying)
            {
                if(obj.GetComponent<AudioSource>().clip.name == gClip)
                {
                    obj.GetComponent<AudioSource>().Stop();
                }
            }
        }
    }

    public void RegisterAudiosource(AudioSource source)
    {
        if(!VolumeGroups.ContainsKey(source.gameObject.tag))
        {
            VolumeGroups.Add(source.gameObject.tag, 1f);
        }

        Instances.Add(source.gameObject);
        source.volume = VolumeGroups[source.gameObject.tag];
    }

    public void MuteMusic()
    {
        SetTemporaryVolume("Music", PlayerPrefs.GetFloat("Music") / 2f);
        //MusicSource.mute = true;
    }

    public void UnmuteMusic()
    {
        ResetTemporaryVolume("Music");
        //MusicSource.mute = false;
    }

    #endregion

    [System.Serializable]
    public class MusicPlaylist
    {
        public string Name;
        public List<string> Playlist = new List<string>();
    }
}
