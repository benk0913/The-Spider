using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FullscreenMissionWindowUI : MonoBehaviour
{
    public static FullscreenMissionWindowUI Instance;

    [SerializeField]
    TextMeshProUGUI MissionTitle;

    [SerializeField]
    TextMeshProUGUI MissionDescription;

    Quest CurrentQuest;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }
    
    private void OnDisable()
    {
        if (AudioControl.Instance != null)
        {
            AudioControl.Instance.StopSound("soundscape_bribing");
            AudioControl.Instance.UnmuteMusic();
        }

        if (MouseLook.Instance != null)
        {
            MouseLook.Instance.CurrentWindow = null;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            this.gameObject.SetActive(false);
        }
    }

    public void Show(Quest quest)
    {
        CurrentQuest = quest;

        MouseLook.Instance.CurrentWindow = this.gameObject;

        AudioControl.Instance.Play("soundscape_bribing", true);
        AudioControl.Instance.MuteMusic();
        
        this.gameObject.SetActive(true);

        RefreshUI();
    }

    void RefreshUI()
    {
        MissionTitle.text = CurrentQuest.name;

        MissionDescription.text = CurrentQuest.Description;
    }

}
