using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOOwnProperty", menuName = "DataObjects/Quests/QuestObjectives/QOOwnProperty", order = 2)]
public class QOOwnProperty : QuestObjective
{
    public Property TargetProperty;

    public override bool Validate()
    {
        if (ParentQuest == null)
        {
            Debug.Log("NO PARENT QUEST ");
            return false;
        }

        if (ParentQuest.ForCharacter == null)
        {
            Debug.Log("NO FOR CHARACTER ");
            return false;
        }


        for (int i=0;i<CORE.Instance.Locations.Count;i++)
        {
            if(CORE.Instance.Locations[i].CurrentProperty != TargetProperty)
            {
                Debug.Log("1 " + CORE.Instance.Locations[i].CurrentProperty.name);
                continue;
            }

            if (CORE.Instance.Locations[i].FactionInControl != ParentQuest.ForCharacter.CurrentFaction)
            {
                Debug.Log("2 " + CORE.Instance.Locations[i].CurrentProperty.name);
                continue;
            }

            Debug.Log("3 " + CORE.Instance.Locations[i].CurrentProperty.name);
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