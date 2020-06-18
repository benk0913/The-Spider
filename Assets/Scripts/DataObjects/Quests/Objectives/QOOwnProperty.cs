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
            if (CORE.Instance.DEBUG)
            {
                Debug.Log("NO FOR CHARACTER - " + this.name);
            }

            return false;
        }

        if (CORE.Instance.Locations.Find(x => 
        x.CurrentProperty == TargetProperty 
        && x.FactionInControl != null 
        && x.FactionInControl == ParentQuest.ForCharacter.CurrentFaction
        && !x.IsDisabled) != null)
        {
            return true;
        }

        return false;
    }

    
}