using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SellItem", menuName = "DataObjects/AgentActions/Item/SellItem", order = 2)]
public class SellItem : AgentAction
{
    public float ValueMultiplier = 0.5f;
    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            return;
        }

        AudioControl.Instance.Play("resource_gold");

        ItemUI itemUI = (ItemUI)target;

        character.TopEmployer.CGold += Mathf.CeilToInt(itemUI.CurrentItem.Price * ValueMultiplier);
        character.TopEmployer.Belogings.Remove(itemUI.CurrentItem);
        InventoryPanelUI.Instance.ItemWasAdded(0);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        reason = null;
        ItemUI item = (ItemUI)target;

        if(!requester.Belogings.Contains(item.CurrentItem))
        {
            return false;
        }

        if (!item.CurrentItem.Sellable)
        {
            reason = new FailReason("This item is not for selling!");
            return false;
        }

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        return true;
    }

    public override List<TooltipBonus> GetBonuses()
    {
        List<TooltipBonus> bonuses = base.GetBonuses();

        //ItemUI target = (ItemUI)RecentTaret;

        //bonuses.Add(new TooltipBonus("Value: " + (target.CurrentItem.Price/2), ResourcesLoader.Instance.GetSprite("receive_money")));
        return bonuses;
    }
}
