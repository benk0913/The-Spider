using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScreenUI : MonoBehaviour
{
    public static TutorialScreenUI Instance;

    public List<TutorialScreenInstance> Instances = new List<TutorialScreenInstance>();

    public TutorialScreenInstance CurrentInstance;

    public Image TutorialScreenImage;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Show(string byKey)
    {
        TutorialScreenInstance instance = Instances.Find(x => x.Key == byKey);

        if(instance == null)
        {
            return;
        }

        CurrentInstance = instance;

        this.gameObject.SetActive(true);

        TutorialScreenImage.sprite = CurrentInstance.Image;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            this.gameObject.SetActive(false);
        }
    }

    [System.Serializable]
    public class TutorialScreenInstance
    {
        public string Key;
        public Sprite Image;
    }
}
