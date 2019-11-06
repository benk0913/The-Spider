using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SellItem", menuName = "DataObjects/AgentActions/Item/SellItem", order = 2)]
public class SellItem : AgentAction
{
    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            return;
        }

        ItemUI itemUI = (ItemUI)target;

        character.TopEmployer.Gold += itemUI.CurrentItem.Price/2;
        character.TopEmployer.Belogings.Remove(itemUI.CurrentItem);
        InventoryPanelUI.Instance.ItemWasAdded(0);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        ItemUI item = (ItemUI)target;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (!item.CurrentItem.Sellable)
        {
            reason = new FailReason("This item is not for selling!");
            return false;
        }

        return true;
    }
}
