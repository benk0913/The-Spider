﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldMissionPanelUI : MonoBehaviour
{
    public static WorldMissionPanelUI Instance;

    [SerializeField]
    TextMeshProUGUI QuestTitle;

    [SerializeField]
    TextMeshProUGUI QuestDescription;

    [SerializeField]
    Transform QuestObjectivesContainer;

    [SerializeField]
    Animator Anim;

    [SerializeField]
    CanvasGroup CG;

    [SerializeField]
    GameObject FoldedPanel;

    Quest CurrentQuest;

    bool isHidden = false;

    public bool ToggleHidden = false;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowQuest(Quest quest)
    {
        CurrentQuest = quest;

        if (!isHidden)
        {
            this.gameObject.SetActive(true);
        }

        Refresh();
    }

    public void HideQuest(Quest quest)
    {
        if(CurrentQuest != quest)
        {
            return;
        }

        CurrentQuest = QuestsPanelUI.Instance.ActiveQuests.Find(x => x.Tutorial);

        if(CurrentQuest != null)
        {
            return;
        }

        Hide();
    }

    public void Refresh()
    {
        if(CurrentQuest == null)
        {
            Hide();
            return;
        }

        QuestTitle.text = CurrentQuest.name;
        QuestDescription.text = CurrentQuest.Description;

        ClearObjectives();
        foreach (QuestObjective objective in CurrentQuest.Objectives)
        {
            TextMeshProUGUI QuestTitle = ResourcesLoader.Instance.GetRecycledObject("ObjectiveTitleUI").GetComponent<TextMeshProUGUI>();
            QuestTitle.transform.SetParent(QuestObjectivesContainer);
            QuestTitle.transform.SetAsLastSibling();
            QuestTitle.text = "<color=" + (objective.IsComplete ? "green" : "yellow") + ">" + objective.name + "</color>";
        }
    }

    void ClearObjectives()
    {
        while(QuestObjectivesContainer.childCount > 0)
        {
            QuestObjectivesContainer.GetChild(0).gameObject.SetActive(false);
            QuestObjectivesContainer.GetChild(0).SetParent(transform);
        }
    }

    public void Hide()
    { 
        if(CurrentQuest != null)
        {
            return;
        }

        this.gameObject.SetActive(false);
    }

    public void SetHidden()
    {
        isHidden = true;
        this.gameObject.SetActive(false);
    }

    public void SetUnHidden()
    {
        isHidden = false;

        if(CurrentQuest == null)
        {
            return;
        }

        this.gameObject.SetActive(true);
    }

    public void QuestComplete()
    {
        Anim.SetTrigger("Complete");
        CurrentQuest = null;
    }

    public  void OnExitAnimationFinished()
    {
        Hide();
    }

    public void ObjectiveComplete()
    {
        Anim.SetTrigger("Notify");

        Refresh();
    }

    public void ToggleHide()
    {
        ToggleHidden = !ToggleHidden;

        CG.alpha = ToggleHidden ? 1f : 0f;
        FoldedPanel.gameObject.SetActive(!ToggleHidden);
    }
}
