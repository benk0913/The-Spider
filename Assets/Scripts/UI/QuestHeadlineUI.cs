using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestHeadlineUI : HeadlineUI
{


    public Quest CurrentQuest;
    
    QuestsPanelUI ParentPanel;

    public void SetInfo(Quest quest, QuestsPanelUI parentPanel)
    {
        CurrentQuest = quest;
        ParentPanel = parentPanel;

        Title.text = CurrentQuest.name;

    }

    public override void Archive()
    {
        Hide();
        this.gameObject.SetActive(false);
        this.transform.SetParent(transform.parent.parent);
        ParentPanel.Complete(CurrentQuest);
    }

    public override void Toggle()
    {
        if (ShowingObject != null)
        {
            Hide();
            return;
        }

        Anim.SetBool("Showing", true);
        ShowingObject = ResourcesLoader.Instance.GetRecycledObject("QuestContentUI");
        ShowingObject.transform.SetParent(transform.parent, false);
        ShowingObject.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
        ShowingObject.GetComponent<QuestContentUI>().SetInfo(CurrentQuest);
    }

    public override void Hide()
    {
        if (ShowingObject == null)
        {
            return;
        }

        ShowingObject.gameObject.SetActive(false);
        ShowingObject.transform.SetParent(transform.parent.parent);
        ShowingObject = null;
        Anim.SetBool("Showing", false);
    }

}
