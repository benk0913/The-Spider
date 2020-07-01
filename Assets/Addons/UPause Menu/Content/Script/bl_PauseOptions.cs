using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class bl_PauseOptions : MonoBehaviour {

    public Transform ResolutionPanel = null;
    public GameObject ResolutionButtons = null;
    [Space(5)]
    public bool ShowFramesPerSecond = true;
    public Text FPSFrames = null;
    public float UpdateInterval = 0.5F;

    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval
    /// <summary>
    /// Get this sensitivity from your mouse loook
    /// </summary>
    public static float Sensitivity = 3f;

    public static bool TutorialOn = true;

    public float Brightness = 1f;

    [SerializeField]
    List<Button> QualityButtons = new List<Button>();

    [SerializeField]
    Slider MouseSensitivity;

    [SerializeField]
    Slider SoundSlider;

    [SerializeField]
    Slider MusicSlider;

    [SerializeField]
    Toggle TutorialOnToggle;

    [SerializeField]
    Slider BrightnessSlider;

    [SerializeField]
    Transform TextureQualityContainer;

    [SerializeField]
    Transform AnisotropicContainer;

    [SerializeField]
    Transform AntiAliasingContainer;

    [SerializeField]
    Transform ShadowCascadesContainer;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        PostResolutions();
        if (FPSFrames != null) { FPSFrames.gameObject.SetActive(ShowFramesPerSecond); }
        timeleft = UpdateInterval;

        QualityButtons.ForEach(x => x.interactable = true);
        QualityButtons[QualitySettings.GetQualityLevel()].interactable = false;
        //Developer Util info
        /*Debug.Log(SystemInfo.deviceModel);
        Debug.Log(SystemInfo.deviceName);
        Debug.Log(SystemInfo.deviceType);
        Debug.Log(SystemInfo.deviceUniqueIdentifier);
        Debug.Log(SystemInfo.graphicsDeviceID);
        Debug.Log(SystemInfo.graphicsDeviceName);
        Debug.Log(SystemInfo.graphicsDeviceVendor);
        Debug.Log(SystemInfo.operatingSystem);
        Debug.Log(SystemInfo.processorCount);
        Debug.Log(SystemInfo.processorType);
        Debug.Log(SystemInfo.systemMemorySize);
        Debug.Log(SystemInfo.supportsShadows);*/
        RefreshTextureQualityUI();
        RefreshAntialiasingUI();
        RefreshAnisotropicUI();
        RefreshShadowCascadesUI();
    }

    
    private void Start()
    {

        MouseSensitivity.value = PlayerPrefs.GetFloat("MouseSensitivity", 3f);
        SoundSlider.value = AudioControl.Instance.VolumeGroups["Untagged"];
        MusicSlider.value = AudioControl.Instance.VolumeGroups["Music"];
        TutorialOn = PlayerPrefs.GetInt("TutorialOn", 1) == 1? true : false;
        TutorialOnToggle.isOn = TutorialOn;
        Brightness = PlayerPrefs.GetFloat("Brightness", Screen.brightness);
        
    }


    /// <summary>
    /// 
    /// </summary>
    void PostResolutions()
    {
        if (ResolutionPanel == null)
            return;
        if (ResolutionButtons == null)
            return;

        for(int i=0;i<ResolutionPanel.childCount;i++)
        {
            Destroy(ResolutionPanel.GetChild(i).gameObject,0.1f);
        }

        List<Resolution> AddedResolutions = new List<Resolution>();
        
        CORE.Instance.DelayedInvokation(1f, () => 
        {
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                bool exists = false;
                foreach(Resolution resolution in AddedResolutions)
                {
                    if(resolution.width == Screen.resolutions[i].width && resolution.height == Screen.resolutions[i].height)
                    {
                        exists = true;
                        break;
                    }
                }

                if(exists)
                {
                    continue;
                }

                AddedResolutions.Add(Screen.resolutions[i]);


                GameObject b = Instantiate(ResolutionButtons) as GameObject;

                b.GetComponentInChildren<Text>().text = Screen.resolutions[i].width + " x " + Screen.resolutions[i].height;
                b.transform.SetParent(ResolutionPanel,false);

                int passedNumber = i;
                AddResolutionListener(b.GetComponent<Button>(), passedNumber);

                if (Screen.width == Screen.resolutions[i].width && Screen.height == Screen.resolutions[i].height)
                {
                    b.GetComponent<Button>().interactable = false;
                }
            }
        });


    }

    void AddResolutionListener(Button butt, int number)
    {
        butt.onClick.AddListener(() => { ChangeResolution(number); });
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (ShowFramesPerSecond && FPSFrames != null)
        {
            FramesPerSecond();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void FramesPerSecond()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;
        
        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            float fps = accum / frames;
            string format = System.String.Format("{0:F2} FPS", fps);
            FPSFrames.text = format;

            if (fps < 30)
            {
                FPSFrames.color = Color.yellow;
            }
            else
            {
                if (fps < 10)
                {
                    FPSFrames.color = Color.red;
                }
                else
                {
                    FPSFrames.color = Color.green;
                }
            }
            timeleft = UpdateInterval;
            accum = 0.0F;
            frames = 0;
        }
    }
    /// <summary>
    /// Change Show / Hide Frames Per Second UI.
    /// </summary>
    /// <param name="b"></param>
    public void ChangeFPSFrames(bool b)
    {
        ShowFramesPerSecond = b;
        FPSFrames.gameObject.SetActive(b);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="r"></param>
    public void ChangeResolution(int r)
    {
        AudioControl.Instance.Play("sound_click");
        Screen.SetResolution(Screen.resolutions[r].width, Screen.resolutions[r].height, true);
        PostResolutions();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="q"></param>
    public void ChangeQuality(int q)
    {
        QualitySettings.SetQualityLevel(q,true);
        AudioControl.Instance.Play("sound_click");

        QualityButtons.ForEach(x => x.interactable = true);
        QualityButtons[q].interactable = false;

        RefreshTextureQualityUI();
        RefreshAntialiasingUI();
        RefreshAnisotropicUI();
        RefreshShadowCascadesUI();
    }

    /// <summary>
    /// Update Volumen of game
    /// </summary>
    /// <param name="v"></param>
    public void UpdateVolumen(float v)
    {
        //Apply volumen
         AudioControl.Instance.SetVolume("Untagged", v);
    }

    public void UpdateVolumenMusic(float v)
    {
        //Apply volumen
        AudioControl.Instance.SetVolume("Music", v);
    }

    /// <summary>
    /// Update Sensitivity of game
    /// </summary>
    /// <param name="v"></param>
    public void UpdateSensitivity(float s)
    {
        Sensitivity = s;
        PlayerPrefs.SetFloat("MouseSensitivity", s);
    }

    public void AntiAliasing(int a)
    {
        QualitySettings.antiAliasing = a;
        AudioControl.Instance.Play("sound_click");

        RefreshAntialiasingUI();
    }

    /// <summary>
    /// Update Texture Quality
    /// </summary>
    /// <param name="tq"></param>
    public void TextureQuality(int tq)
    {
        QualitySettings.masterTextureLimit = tq;
        AudioControl.Instance.Play("sound_click");

        RefreshTextureQualityUI();
    }

    /// <summary>
    /// Update Anisotropic Texture
    /// </summary>
    /// <param name="a"></param>
    public void UpdateAnisotropic(int a)
    {
        AudioControl.Instance.Play("sound_click");

        switch (a)
        {
            case 0:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            break;
            case 1:
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
            break;
            case 2:
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
            break;
        }

        RefreshAnisotropicUI();
    }
    /// <summary>
    /// Update VSync Option
    /// </summary>
    /// <param name="vs"></param>
    public void VSync(int vs)
    {
        AudioControl.Instance.Play("sound_click");
        QualitySettings.vSyncCount = vs;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="s"></param>
    public void ShadowsCascades(int s)
    {
        AudioControl.Instance.Play("sound_click");
        QualitySettings.shadowCascades = s;

        RefreshShadowCascadesUI();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="bw"></param>
    public void BlendWight(int bw)
    {
        switch (bw)
        {
            case 0 :
                QualitySettings.skinWeights = SkinWeights.OneBone;
                break;
            case 1:
                QualitySettings.skinWeights = SkinWeights.TwoBones;
                break;
            case 2:
                QualitySettings.skinWeights = SkinWeights.FourBones;
                break;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    public void SoftVegetation(bool b)
    {
        QualitySettings.softVegetation = b;
    }

    public void SetTutorialScreens(bool b)
    {
        TutorialOn = b;
        PlayerPrefs.SetInt("TutorialOn", TutorialOn ? 1 : 0);
    }

    public void ResetTutorial()
    {
        TutorialScreenUI.Instance.ResetTutorial();
    }

    public void UpdateBrightness(float v)
    {
        Brightness = v;
        Screen.brightness = Brightness;
        PlayerPrefs.SetFloat("Brightness", Brightness);
    }

    public void RefreshTextureQualityUI()
    {
        for(int i=0;i<TextureQualityContainer.childCount;i++)
        {
            TextureQualityContainer.GetChild(i).GetComponent<Button>().interactable = QualitySettings.masterTextureLimit == i? false : true;
        }   
    }

    public void RefreshShadowCascadesUI()
    {
        for (int i = 0; i < ShadowCascadesContainer.childCount; i++)
        {
            ShadowCascadesContainer.GetChild(i).GetComponent<Button>().interactable = QualitySettings.shadowCascades == i ? false : true;
        }
    }

    public void RefreshAntialiasingUI()
    {
        for (int i = 0; i < AntiAliasingContainer.childCount; i++)
        {
            AntiAliasingContainer.GetChild(i).GetComponent<Button>().interactable = QualitySettings.antiAliasing == i ? false : true;
        }
    }

    public void RefreshAnisotropicUI()
    {
        for (int i = 0; i < AnisotropicContainer.childCount; i++)
        {
            AnisotropicContainer.GetChild(i).GetComponent<Button>().interactable = (int)QualitySettings.anisotropicFiltering == i ? false : true;
        }
    }
}
