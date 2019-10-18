using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOEventInvoked", menuName = "DataObjects/Quests/QuestObjectives/QOEventInvoked", order = 2)]
public class QOEventInvoked : QuestObjective
{
    bool valid = false;
    bool subscribed = false;

    public string EventKey = "PassTimeComplete";

    public override bool Validate()
    {
        if (!subscribed)
        {
            CORE.Instance.SubscribeToEvent(EventKey, OnEventInvoked);
        }

        if(valid)
        {
            subscribed = false;
            CORE.Instance.UnsubscribeFromEvent(EventKey, OnEventInvoked);
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnEventInvoked()
    {
        valid = true;
    }
}