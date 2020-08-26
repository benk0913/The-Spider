using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDCHasItem", menuName = "DataObjects/Dialog/Conditions/DDCHasItem", order = 2)]
public class DDCHasItem : DialogDecisionCondition
{
    public Item TheItem;

    public int Amount = 1;

    public override bool CheckCondition()
    {
        if (Amount == 1)
        {
            if (InventoryPanelUI.Instance.GetItem(TheItem.name) == null)
            {
                if (Inverted)
                {
                    return base.CheckCondition();
                }

                return false;
            }

            return base.CheckCondition();
        }

        List<Item> items = CORE.PC.Belogings.FindAll(X => X.name == TheItem.name);
        if(items.Count >= Amount)
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
