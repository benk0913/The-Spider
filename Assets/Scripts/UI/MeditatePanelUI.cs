using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeditatePanelUI : MonoBehaviour
{
    public static MeditatePanelUI Instance;

    public Transform Container;

    public List<string> GoodSentences = new List<string>();
    public List<string> BadSentences  = new List<string>();

    public Animator Animer;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }


    //void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        AnimateExit();
    //    }
    //}

    void Hide()
    {
        MeditationRoutineInstance = null;
        this.gameObject.SetActive(false);
    }

    public void AnimateExit()
    {
        Animer.SetBool("Exit", true);
    }

    public void Show()
    {
        Animer.SetBool("Exit", false);
        Animer.SetTrigger("Reset");
        this.gameObject.SetActive(true);


        if(MeditationRoutineInstance != null)
        {
            StopCoroutine(MeditationRoutineInstance);
        }

        MeditationRoutineInstance = StartCoroutine(MeditationRoutine());
    }

    public void PressedOnGood()
    {
        AudioControl.Instance.Play("quest_complete");

        if(Random.Range(0,2) == 0)
        {
            return;
        }

        CORE.Instance.PsychoEffectRate--;
        if(CORE.Instance.PsychoEffectRate == 0)
        {
            AnimateExit();
            GlobalMessagePrompterUI.Instance.Show("You feel better...", 3f, Color.green);
        }
    }

    public void PressedOnBad()
    {
        AudioControl.Instance.Play("researchchar_bad");

        if (Random.Range(0, 2) == 0)
        {
            return;
        }

        CORE.Instance.PsychoEffectRate++;
        if (CORE.Instance.PsychoEffectRate >= 10)
        {
            GlobalMessagePrompterUI.Instance.Show("You feel worse...", 1f, Color.red);
        }
    }

    public void ClearContainer()
    {
        while(Container.childCount > 0)
        {
            Container.GetChild(0).gameObject.SetActive(false);
            Container.GetChild(0).SetParent(transform);
        }
    }

    Coroutine MeditationRoutineInstance;

    IEnumerator MeditationRoutine()
    {
        MeditateSentenceUI sentence;
        while (CORE.Instance.PsychoEffectRate > 0)
        {
            yield return new WaitForSeconds(2f / (CORE.Instance.PsychoEffectRate + 1f));

            sentence = ResourcesLoader.Instance.GetRecycledObject("MeditateTextUI").GetComponent<MeditateSentenceUI>();
            sentence.transform.SetParent(Container);
            sentence.transform.position = new Vector2(Random.Range(0, Screen.width), Random.Range(0, Screen.height));

            if (Random.Range(0, 2) == 0)
            {
                sentence.Show(GoodSentences[Random.Range(0, GoodSentences.Count)], PressedOnGood);
            }
            else
            {
                sentence.Show(BadSentences[Random.Range(0, GoodSentences.Count)], PressedOnBad);
            }
        }

        MeditationRoutineInstance = null;
        AnimateExit();
    }
}
