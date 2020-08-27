﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestHeadlineUI : HeadlineUI
{
    public Quest CurrentQuest;
    
    QuestsPanelUI ParentPanel;

    [SerializeField]
    GameObject QuestTutorialButton;

    [SerializeField]
    GameObject QuestGlowObject;

    public void SetInfo(Quest quest, QuestsPanelUI parentPanel)
    {
        CurrentQuest = quest;
        ParentPanel = parentPanel;

        Title.text = CurrentQuest.name;

        if (CurrentQuest != null)
        {
            QuestTutorialButton.SetActive(CurrentQuest.RelevantTutorial != null);
        }

        QuestGlowObject.SetActive(CurrentQuest.GlowHeadline);
    }

    public override void Toggle()
    {
        if (ShowingObject != null)
        {
            Hide();
            return;
        }

        Show();
    }

    public virtual void Show()
    {
        if(ShowingObject != null)
        {
            return;
        }

        Anim.SetBool("Showing", true);
        ShowingObject = ResourcesLoader.Instance.GetRecycledObject("QuestContentUI");
        ShowingObject.transform.SetParent(transform.parent, false);
        ShowingObject.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
        ShowingObject.GetComponent<QuestContentUI>().SetInfo(this);

        if (CurrentQuest != null)
        {
            QuestTutorialButton.SetActive(CurrentQuest.RelevantTutorial != null);
        }
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

        RightClickDropDownPanelUI.Instance.Hide();
    }

    public void Complete()
    {
        Show();
        ShowingObject.GetComponent<QuestContentUI>().Complete();
        ShowingObject = null;
    }

    public void SelfArchive()
    {

        this.gameObject.SetActive(false);
    }
    
    public void ShowQuestTutorial()
    {
        if(CurrentQuest == null || CurrentQuest.RelevantTutorial == null)
        {
            return;
        }

        TutorialScreenUI.Instance.Show(CurrentQuest.RelevantTutorial.name,0f,true);
    }

}
