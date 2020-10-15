using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOHaveSessionRule", menuName = "DataObjects/Quests/QuestObjectives/QOHaveSessionRule", order = 2)]
public class QOHaveSessionRule : QuestObjective
{
    public SessionRule TargetRule;

    public override bool Validate()
    {
        if(ParentQuest.ForCharacter == null)
        {
            return false;
        }

        if(TargetRule == null)
        {
            Debug.LogError("NO TARGET RULE " + this.name);
        }

        if(CORE.Instance.SessionRules.Rules.Find(x => x.name == TargetRule.name) == null)
        {
            return false;
        }

        return true;
    }
    
}