using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOHasEmployeesCount", menuName = "DataObjects/Quests/QuestObjectives/QOHasEmployeesCount", order = 2)]
public class QOHasEmployeesCount : QuestObjective
{
    bool valid = false;
    public int count = 1;

    int currentCount = 0;

    public override bool Validate()
    {
        return CORE.PC.CharactersInCommand.Count > count;
    }

    void OnEventInvoked()
    {
        valid = true;
    }
}