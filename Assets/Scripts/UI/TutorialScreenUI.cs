using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScreenUI : MonoBehaviour
{
    public static TutorialScreenUI Instance;

    public TutorialScreenInstance CurrentInstance;

    public Image TutorialScreenImage;

    public List<TutorialScreenInstance> Instances = new List<TutorialScreenInstance>();

    [SerializeField]
    CanvasGroup CG;

    private void Awake()
    {
        Instance = this;
        StartCoroutine(Init());
        CG.alpha = 0f;
    }

    IEnumerator Init()
    {

        while(CORE.Instance == null)
        {
            yield return 0;
        }

        CORE.Instance.Database.TutorialScreenInstances.ForEach(x => Instances.Add(x.Clone()));
        Instances.ForEach(x => x.WasSeen = (PlayerPrefs.GetInt("tut_" + x.name, 0) == 1? true : false));
        CG.alpha = 1f;
        this.gameObject.SetActive(false);
    }

    public void Show(string byKey, float invokationDelay = 1f)
    {
            if (!bl_PauseOptions.TutorialOn)
            {
                return;
            }

            TutorialScreenInstance instance = Instances.Find(x => x.name == byKey);

            if (instance == null)
            {
                return;
            }

            if (instance.WasSeen)
            {
                return;
            }

        CORE.Instance.DelayedInvokation(invokationDelay, () =>
        {
            CG.alpha = 0f;

            CurrentInstance = instance;

            this.gameObject.SetActive(true);

            TutorialScreenImage.sprite = CurrentInstance.Image;

            instance.WasSeen = true;
            PlayerPrefs.SetInt("tut_" + instance.name, 1);
        });
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Continue();
        }
    }

    void Hide()
    {
        this.gameObject.SetActive(false);
    }

    void Continue()
    {
        if(!string.IsNullOrEmpty(CurrentInstance.NextScreenKey))
        {
            Instances.Find(x => x.name == CurrentInstance.NextScreenKey).WasSeen = false;
            Show(CurrentInstance.NextScreenKey,0f);
            return;
        }

        Hide();
    }

    public void ResetTutorial()
    {
        WarningWindowUI.Instance.Show("Are you sure you want to reset the tutorial screens?", () =>
         {
             Instances.ForEach((x) =>
             {
                 x.WasSeen = false;
                 PlayerPrefs.SetInt("tut_" + x.name, 0);
             });
         });
    }
}
