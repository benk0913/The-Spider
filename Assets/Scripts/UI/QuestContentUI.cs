using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestContentUI : HeadlineContentUI
{
    public Quest CurrentQuest;


    public void SetInfo(Quest quest)
    {
        CurrentQuest = quest;

        ContentText.text = CurrentQuest.Description;

        if (CurrentQuest.Icon != null)
        {
            Icon.sprite = CurrentQuest.Icon;
        }
        else
        {
            Icon.sprite = DefaultIcon;
        }
    }
}
