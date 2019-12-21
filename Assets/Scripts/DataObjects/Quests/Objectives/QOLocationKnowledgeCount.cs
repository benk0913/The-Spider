using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOLocationKnowledgeCount", menuName = "DataObjects/Quests/QuestObjectives/QOLocationKnowledgeCount", order = 2)]
public class QOLocationKnowledgeCount : QuestObjective
{
    bool valid = false;
    bool subscribed = false;

    public string KnowledgeKey;
    public int Count = 1;

    public override bool Validate()
    {
        if(CORE.Instance.Locations.FindAll(x => x.Known.IsKnown(KnowledgeKey, CORE.PC)).Count >= Count)
        {
            return true;
        }

        return false;
    }
}