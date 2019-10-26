using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestContentUI : HeadlineContentUI
{
    public Quest CurrentQuest;

    [SerializeField]
    PortraitUI CharacterPortrait;

    [SerializeField]
    Transform ObjectivesContainer;

    QuestHeadlineUI CurrentHeadline;

    public void SetInfo(QuestHeadlineUI headline)
    {
        CurrentHeadline = headline;
        CurrentQuest = CurrentHeadline.CurrentQuest;
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

        ClearObjectivesContainer();

        foreach(QuestObjective objective in CurrentQuest.Objectives)
        {
            TextMeshProUGUI QuestTitle = ResourcesLoader.Instance.GetRecycledObject("ObjectiveTitleUI").GetComponent<TextMeshProUGUI>();
            QuestTitle.transform.SetParent(ObjectivesContainer);
            QuestTitle.transform.SetAsLastSibling();
            QuestTitle.transform.localScale = Vector3.one;
            QuestTitle.text = "<color=" + (objective.IsComplete ? "green" : "yellow") + ">" + objective.name + "</color>";
        }
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
        CurrentHeadline.SelfArchive();
    }

    void ClearObjectivesContainer()
    {
        while(ObjectivesContainer.childCount > 0)
        {
            ObjectivesContainer.GetChild(0).gameObject.SetActive(false);
            ObjectivesContainer.GetChild(0).SetParent(transform);
        }
    }
}
