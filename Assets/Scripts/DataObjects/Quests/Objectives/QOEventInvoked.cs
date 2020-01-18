using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOEventInvoked", menuName = "DataObjects/Quests/QuestObjectives/QOEventInvoked", order = 2)]
public class QOEventInvoked : QuestObjective
{
    bool valid = false;
    bool subscribed = false;
    public int repeats = 1;

    public string EventKey = "PassTimeComplete";

    int currentCount = 0;

    public override bool Validate()
    {
        if (!subscribed)
        {
            subscribed = true;
            CORE.Instance.SubscribeToEvent(EventKey, OnEventInvoked);
        }

        if(valid)
        {
            currentCount++;

            if(currentCount < repeats)
            {
                valid = false;
                return false;
            }

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