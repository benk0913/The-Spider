using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDCCanBuy", menuName = "DataObjects/Dialog/Conditions/DDCCanBuy", order = 2)]
public class DDCCanBuy : DialogDecisionCondition
{
    public Item TheItem;

    public override bool CheckCondition()
    {
        if(CORE.PC.Gold < TheItem.Price)
        {
            if(Inverted)
            {
                return base.CheckCondition();
            }

            return false;
        }

        return base.CheckCondition();
    }
}
