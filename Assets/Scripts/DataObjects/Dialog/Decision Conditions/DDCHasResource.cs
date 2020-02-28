using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDCHasResource", menuName = "DataObjects/Dialog/Conditions/DDCHasResource", order = 2)]
public class DDCHasResource : DialogDecisionCondition
{
    public int Gold;
    public int Rumors;
    public int Connections;
    public int Progression;

    public override bool CheckCondition()
    {
        if (CORE.PC.Gold >= Gold && CORE.PC.Rumors >= Rumors && CORE.PC.Connections >= Connections && CORE.PC.Progress >= Progression)
        {
            if (Inverted)
            {
                return base.CheckCondition();
            }

            return false;
        }

        return base.CheckCondition();
    }
}
