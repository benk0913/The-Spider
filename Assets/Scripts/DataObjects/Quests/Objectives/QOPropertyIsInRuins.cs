using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOPropertyIsInRuins", menuName = "DataObjects/Quests/QuestObjectives/QOPropertyIsInRuins", order = 2)]
public class QOPropertyIsInRuins : QuestObjective
{
    public Property TargetProperty;

    LocationEntity TheLocation;

    public override bool Validate()
    {
        if (ParentQuest.ForCharacter == null)
        {
            Debug.Log("NO FOR CHARACTER ");
            return false;
        }

        if (TheLocation == null)
        {
            TheLocation = CORE.Instance.Locations.Find(x => x.CurrentProperty == TargetProperty);
        }

        if(TheLocation.IsRuined)
        {
            return true;
        }

        return false;
    }

    
}