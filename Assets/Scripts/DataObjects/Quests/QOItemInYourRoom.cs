using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOItemInYourRoom", menuName = "DataObjects/Quests/QuestObjectives/QOItemInYourRoom", order = 2)]
public class QOItemInYourRoom : QuestObjective
{
    public Item TargetItem;

    public override bool Validate()
    {
        return RoomsManager.Instance.GetItem(TargetItem.RealWorldPrefab.name) != null;
    }
    
}