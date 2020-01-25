using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOResearchComplete", menuName = "DataObjects/Quests/QuestObjectives/QOResearchComplete", order = 2)]
public class QOResearchComplete : QuestObjective
{
    bool valid = false;
    bool subscribed = false;

    public TechTreeItem TechRequired;

    public override bool Validate()
    {
        if (!subscribed)
        {
            subscribed = true;
            CORE.Instance.SubscribeToEvent("ResearchComplete", OnEventInvoked);
            OnEventInvoked();
        }

        if(valid)
        {
            subscribed = false;
            CORE.Instance.UnsubscribeFromEvent("ResearchComplete", OnEventInvoked);
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnEventInvoked()
    {
        TechTreeItem item = CORE.Instance.TechTree.Find(x => x.name == TechRequired.name);

        if (item == null)
        {
            return;
        }

        if(!item.IsResearched)
        {
            return;
        }

        valid = true;
    }
}