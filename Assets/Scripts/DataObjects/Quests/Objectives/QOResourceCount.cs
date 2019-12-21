using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOResourceCount", menuName = "DataObjects/Quests/QuestObjectives/QOResourceCount", order = 2)]
public class QOResourceCount : QuestObjective
{

    public int Gold;
    public int Connections;
    public int Rumors;
    public int Progression;

    public override bool Validate()
    {
        if(Gold > 0 && CORE.PC.Gold >= Gold)
        {
            return true;
        }

        if (Rumors > 0 && CORE.PC.Rumors >= Rumors)
        {
            return true;
        }

        if (Connections > 0 && CORE.PC.Connections >= Connections)
        {
            return true;
        }

        if (Progression > 0 && CORE.PC.Progress >= Progression)
        {
            return true;
        }

        return false;
    }
}