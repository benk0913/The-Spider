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
        if (CORE.PC.CGold >= Gold && CORE.PC.CRumors >= Rumors && CORE.PC.CConnections >= Connections && CORE.PC.CProgress >= Progression)
        {
            if (Inverted)
            {
                return false;
            }

            return base.CheckCondition();
        }

        if (Inverted)
        {
            return base.CheckCondition();
        }

        return false;
    }
}
