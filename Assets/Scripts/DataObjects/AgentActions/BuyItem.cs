using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BuyItem", menuName = "DataObjects/AgentActions/Item/BuyItem", order = 2)]
public class BuyItem : AgentAction
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

        character.TopEmployer.Gold -= itemUI.CurrentItem.Price;
        character.TopEmployer.Belogings.Add(itemUI.CurrentItem);
        itemUI.LocationParent.Inventory.Remove(itemUI.CurrentItem);
        InventoryPanelUI.Instance.ItemWasAdded(0);

        InventoryPanelUI.Instance.RefreshInventory();
        SelectedPanelUI.Instance.LocationPanel.RefreshInventory();
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        ItemUI item = (ItemUI)target;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if(item.LocationParent == null)
        {
            return false;
        }

        if(!item.LocationParent.CurrentProperty.IsVendor)
        {
            return false;
        }

        if (!item.CurrentItem.Sellable)
        {
            reason = new FailReason("This item is not for selling!");
            return false;
        }

        if(requester.Gold < item.CurrentItem.Price)
        {
            reason = new FailReason("Not enough gold!");
            return false;
        }

        return true;
    }

    public override List<TooltipBonus> GetBonuses()
    {
        List<TooltipBonus> bonuses = base.GetBonuses();

        if (RecentTaret != null)
        {
            ItemUI target = (ItemUI)RecentTaret;

            bonuses.Add(new TooltipBonus("Price: " + target.CurrentItem.Price, ResourcesLoader.Instance.GetSprite("pay_money")));
        }

        return bonuses;
    }
}
