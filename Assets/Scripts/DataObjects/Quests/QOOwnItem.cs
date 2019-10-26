﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOOwnItem", menuName = "DataObjects/Quests/QuestObjectives/QOOwnItem", order = 2)]
public class QOOwnItem : QuestObjective
{
    public Item TargetItem;

    public override bool Validate()
    {
        return CORE.PC.GetItem(TargetItem.name) != null;
    }
    
}