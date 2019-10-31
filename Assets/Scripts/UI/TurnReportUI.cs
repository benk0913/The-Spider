using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnReportUI : MonoBehaviour
{
    public static TurnReportUI Instance;

    [SerializeField]
    Transform Container;

    [SerializeField]
    Animator Anim;

    [SerializeField]
    Toggle Toggler;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("PassTimeComplete", Refresh);
    }

    public List<TurnReportLogItemInstance> Log = new List<TurnReportLogItemInstance>();

    public void Refresh()
    {
        Invoke("DelayedRefresh", 0.05f);
    }

    void DelayedRefresh()
    {
        while (Container.childCount > 0)
        {
            Container.GetChild(0).gameObject.SetActive(false);
            Container.GetChild(0).SetParent(transform);
        }

        if (Log.Count == 0)
        {
            return;
        }

        foreach (TurnReportLogItemInstance logitem in Log)
        {
            GameObject turnReportItem = ResourcesLoader.Instance.GetRecycledObject("TurnReportItemUI");
            turnReportItem.transform.SetParent(Container, false);
            turnReportItem.GetComponent<TurnReportItemUI>().SetInfo(logitem);
        }

        Log.Clear();
    }

    public void Toggle()
    {
        if(Toggler.isOn)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void Show()
    {
        Anim.SetTrigger("Show");
    }

    public void Hide()
    {
        Anim.SetTrigger("Hide");
    }
}

public class TurnReportLogItemInstance
{
    public string Content;
    public Sprite Icon;
    public Character RelevantCharacter;

    public TurnReportLogItemInstance(string content = "", Sprite icon = null, Character relevantCharacter = null)
    {
        this.Content = content;
        this.Icon = icon;
        this.RelevantCharacter = relevantCharacter;
    }
}