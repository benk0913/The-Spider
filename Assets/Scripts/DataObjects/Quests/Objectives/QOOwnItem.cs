using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOOwnItem", menuName = "DataObjects/Quests/QuestObjectives/QOOwnItem", order = 2)]
public class QOOwnItem : QuestObjective
{
    public Item TargetItem;

    public int RequestedAmount = 1;

    public override bool Validate()
    {
        if(ParentQuest.ForCharacter == null)
        {
            return false;
        }

        List<Item> items = ParentQuest.ForCharacter.Belogings.FindAll(x => x.name == TargetItem.name);

        if(items == null || items.Count == 0)
        {
            return false;
        }

        if(items.Count < RequestedAmount)
        {
            return false;
        }

        return true;
    }
    
}