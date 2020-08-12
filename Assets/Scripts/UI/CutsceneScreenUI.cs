using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutsceneScreenUI : MonoBehaviour
{
    public static CutsceneScreenUI Instance;

    public VideoPlayer TutorialVideo;

    System.Action OnComplete;


    private void Awake()
    {
        Instance = this;
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {

        while(CORE.Instance == null)
        {
            yield return 0;
        }
        
        this.gameObject.SetActive(false);
    }

    public bool FirstPrep = false;

    public void Show(VideoClip video, System.Action onComplete)
    {

        this.gameObject.SetActive(true);

        FirstPrep = true;

        TutorialVideo.clip = video; 
        TutorialVideo.Prepare();
        TutorialVideo.prepareCompleted += delegate { TutorialVideo.Play(); FirstPrep = false ; };
        
        

        OnComplete = onComplete;

        AudioControl.Instance.MuteMusic();
    }

    private void LateUpdate()
    {
        if((!FirstPrep && TutorialVideo.isPrepared && !TutorialVideo.isPlaying)
            || Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.Escape)
            || Input.GetKeyDown(KeyCode.KeypadEnter) 
            || Input.GetKeyDown(KeyCode.Return))
        {
            Hide();
        }
    }

    void Hide()
    {
        AudioControl.Instance.UnmuteMusic();
        OnComplete?.Invoke();
        TutorialVideo.Stop();
        this.gameObject.SetActive(false);
    }
    
}
