using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDCHasSessionRule", menuName = "DataObjects/Dialog/Conditions/DDCHasSessionRule", order = 2)]
public class DDCHasSessionRule : DialogDecisionCondition
{
    public SessionRule Rule;

    public override bool CheckCondition()
    {
        if(CORE.Instance.SessionRules.Rules.Find(x=>Rule.name == x.name) == null)
        {
            if(Inverted)
            {
                return base.CheckCondition();
            }

            return false;
        }

        if (Inverted)
        {
            return false;
        }

        return base.CheckCondition();
    }
}
