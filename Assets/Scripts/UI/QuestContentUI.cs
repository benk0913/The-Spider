﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestContentUI : HeadlineContentUI
{
    public Quest CurrentQuest;

    [SerializeField]
    PortraitUI CharacterPortrait;

    [SerializeField]
    LocationPortraitUI LocationPortrait;

    [SerializeField]
    Transform ObjectivesContainer;

    [SerializeField]
    Transform RewardsContainer;

    [SerializeField]
    GameObject RewardsTitle;

    [SerializeField]
    ScrollRect Scroll;

    QuestHeadlineUI CurrentHeadline;

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("PassTimeComplete", Refresh);
        CORE.Instance.SubscribeToEvent("HideResearchCharacterWindow", Refresh);
    }

    public void SetInfo(QuestHeadlineUI headline)
    {
        CurrentHeadline = headline;
        CurrentQuest = CurrentHeadline.CurrentQuest;
        Refresh();
    }

    public void SetInfo(Quest quest)
    {
        CurrentQuest = quest;
        Refresh();
    }

    public void Refresh()
    {
        if(CurrentQuest == null)
        {
            return;
        }

        ContentText.text = CurrentQuest.Description;

        if (CurrentQuest.Icon != null)
        {
            Icon.sprite = CurrentQuest.Icon;
        }
        else
        {
            Icon.sprite = DefaultIcon;
        }

        if (CurrentQuest.RelevantCharacter == null)
        {
            CharacterPortrait.gameObject.SetActive(false);
        }
        else
        {
            CharacterPortrait.gameObject.SetActive(true);
            CharacterPortrait.SetCharacter(CurrentQuest.RelevantCharacter);
        }

        if (CurrentQuest.RelevantLocation == null)
        {
            LocationPortrait.gameObject.SetActive(false);
        }
        else
        {
            LocationPortrait.gameObject.SetActive(true);
            LocationPortrait.SetLocation(CurrentQuest.RelevantLocation);
        }

        ClearContainers();

        foreach(QuestObjective objective in CurrentQuest.Objectives)
        {
            TextMeshProUGUI QuestTitle = ResourcesLoader.Instance.GetRecycledObject("ObjectiveTitleUI").GetComponent<TextMeshProUGUI>();
            QuestTitle.transform.SetParent(ObjectivesContainer);
            QuestTitle.transform.SetAsLastSibling();
            QuestTitle.transform.localScale = Vector3.one;
            QuestTitle.text = "<color=" + (objective.IsComplete ? "green" : "black") + ">" + objective.name + "</color>";
        }


        RewardsTitle.gameObject.SetActive(CurrentQuest.Rewards.Length > 0);

        foreach (QuestReward reward in CurrentQuest.Rewards)
        {
            TextMeshProUGUI QuestTitle = ResourcesLoader.Instance.GetRecycledObject("ObjectiveTitleUI").GetComponent<TextMeshProUGUI>();
            QuestTitle.transform.SetParent(RewardsContainer);
            QuestTitle.transform.SetAsLastSibling();
            QuestTitle.transform.localScale = Vector3.one;
            QuestTitle.text = "<color=black>" + reward.name + "</color>";
        }

        Scroll.verticalNormalizedPosition = 1f;

    }

    public void Notify()
    {
        Anim.SetTrigger("Notify");
    }

    public void Complete()
    {
        Anim.SetTrigger("Complete");
    }

    public void OnExitAnimationFinished()
    {
        this.gameObject.SetActive(false);

        if(CurrentHeadline != null)
            CurrentHeadline.SelfArchive();
    }

    void ClearContainers()
    {
        while(ObjectivesContainer.childCount > 0)
        {
            ObjectivesContainer.GetChild(0).gameObject.SetActive(false);
            ObjectivesContainer.GetChild(0).SetParent(transform);
        }

        while (RewardsContainer.childCount > 0)
        {
            RewardsContainer.GetChild(0).gameObject.SetActive(false);
            RewardsContainer.GetChild(0).SetParent(transform);
        }
    }
}
