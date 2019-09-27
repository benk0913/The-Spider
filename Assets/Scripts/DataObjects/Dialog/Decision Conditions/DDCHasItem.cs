using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDCHasItem", menuName = "DataObjects/Dialog/Conditions/DDCHasItem", order = 2)]
public class DDCHasItem : DialogDecisionCondition
{
    public Item TheItem;

    public override bool CheckCondition()
    {
        if(InventoryPanelUI.Instance.GetItem(TheItem.name) == null)
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
