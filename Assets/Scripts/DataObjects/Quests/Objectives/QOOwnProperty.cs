using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOOwnProperty", menuName = "DataObjects/Quests/QuestObjectives/QOOwnProperty", order = 2)]
public class QOOwnProperty : QuestObjective
{
    public Property TargetProperty;

    public override bool Validate()
    {
        if (ParentQuest.ForCharacter == null)
        {
            Debug.Log("NO FOR CHARACTER ");
            return false;
        }

        if (CORE.Instance.Locations.Find(x => 
        x.CurrentProperty == TargetProperty 
        && x.FactionInControl != null 
        && x.FactionInControl == ParentQuest.ForCharacter.CurrentFaction) != null)
        {
            return true;
        }

        return false;
    }

    
}