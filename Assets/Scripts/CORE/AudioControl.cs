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

    public void PlayInPosition(string gClip, Vector3 pos, float MaxDistance = 47f)
    {
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

        currentInstance.transform.position = pos;
        currentInstance.GetComponent<AudioSource>().spatialBlend = 1f;
        currentInstance.GetComponent<AudioSource>().maxDistance = MaxDistance;
        currentInstance.GetComponent<AudioSource>().pitch = 1f;
        currentInstance.GetComponent<AudioSource>().clip = ResourcesLoader.Instance.GetClip(gClip);
        currentInstance.GetComponent<AudioSource>().Play();

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
            currentInstance = (GameObject)Instantiate(ResourcesLoader.Instance.GetObject(m_sInstancePrefab));
            currentInstance.transform.parent = m_tInstancesContainer;
            Instances.Add(currentInstance);
        }

        currentInstance.GetComponent<AudioSource>().spatialBlend = 0f;
        currentInstance.GetComponent<AudioSource>().pitch = 1f;
        currentInstance.GetComponent<AudioSource>().clip = ResourcesLoader.Instance.GetClip(gClip);
        currentInstance.GetComponent<AudioSource>().Play();

        if (VolumeGroups.ContainsKey(currentInstance.tag))
        {
            currentInstance.GetComponent<AudioSource>().volume = VolumeGroups[currentInstance.tag];
        }
    }

    public void Play(string gClip, bool gLoop)
    {
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
        currentInstance.GetComponent<AudioSource>().pitch = 1f;
        currentInstance.GetComponent<AudioSource>().loop = gLoop;
        currentInstance.GetComponent<AudioSource>().clip = ResourcesLoader.Instance.GetClip(gClip);
        currentInstance.GetComponent<AudioSource>().Play();

        if (VolumeGroups.ContainsKey(currentInstance.tag))
        {
            currentInstance.GetComponent<AudioSource>().volume = VolumeGroups[currentInstance.tag];
        }

    }

    public void Play(string gClip, bool gLoop, string gTag)
    {
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
        currentInstance.GetComponent<AudioSource>().pitch = 1f;
        currentInstance.GetComponent<AudioSource>().loop = gLoop;
        currentInstance.GetComponent<AudioSource>().clip = ResourcesLoader.Instance.GetClip(gClip);
        currentInstance.GetComponent<AudioSource>().Play();

        currentInstance.tag = gTag;

        if(VolumeGroups.ContainsKey(currentInstance.tag))
        {
            currentInstance.GetComponent<AudioSource>().volume = VolumeGroups[currentInstance.tag];
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

    public void SetMasterVolume(float gVolume)
    {
        AudioListener.volume = gVolume;
        PlayerPrefs.SetFloat("MasterVolume", gVolume);
        PlayerPrefs.Save();
    }

    public void PlayWithPitch(string gClip,float fPitch)
    {
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
        currentInstance.GetComponent<AudioSource>().Play();

        if (VolumeGroups.ContainsKey(currentInstance.tag))
        {
            currentInstance.GetComponent<AudioSource>().volume = VolumeGroups[currentInstance.tag];
        }
    }

    public void SetMusic(string gClip, float fPitch = 1f)
    {
        MusicSource.volume = VolumeGroups["Music"];

        if (string.IsNullOrEmpty(gClip))
        {
            MusicSource.Stop();
            MusicSource.clip = null;
            return;
        }

        MusicSource.pitch = fPitch;

        if(MusicSource.clip == null || MusicSource.clip.name != gClip)
        {
            MusicSource.Stop();
            MusicSource.clip = ResourcesLoader.Instance.GetClip(gClip);
            MusicSource.Play();
        }
    }

    public void StopSound(string gClip)
    {
        foreach(GameObject obj in Instances)
        {
            if(obj.GetComponent<AudioSource>().isPlaying)
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
    #endregion
}
