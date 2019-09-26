using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestContentUI : HeadlineContentUI
{
    public Quest CurrentQuest;

    [SerializeField]
    PortraitUI CharacterPortrait;

    private void OnEnable()
    {
        GameClock.Instance.OnTurnPassed.AddListener(Refresh);
    }

    private void OnDisable()
    {
        GameClock.Instance.OnTurnPassed.RemoveListener(Refresh);
    }

    public void SetInfo(Quest quest)
    {
        CurrentQuest = quest;
        Refresh();
    }

    void Refresh()
    {
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
    }
}
