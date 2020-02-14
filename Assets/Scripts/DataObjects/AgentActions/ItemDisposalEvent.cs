using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ItemDisposalEvent", menuName = "DataObjects/AgentActions/Item/ItemDisposalEvent", order = 2)]
public class ItemDisposalEvent : AgentAction
{
    public string InvokeEventKey;

    public List<Item> OtherRequiredItems = new List<Item>();

    public List<Item> CanBeOnlyDoneWithItems = new List<Item>();

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            return;
        }

        ItemUI itemUI = (ItemUI)target;

        character.TopEmployer.Belogings.Remove(itemUI.CurrentItem);
        InventoryPanelUI.Instance.ItemWasAdded(0);

        CORE.Instance.InvokeEvent(InvokeEventKey);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        ItemUI item = (ItemUI)target;

        reason = null;

        if(CanBeOnlyDoneWithItems.Find(x=>x.name == item.CurrentItem.name) == null)
        {
            return false;
        }

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        foreach(Item reqItem in OtherRequiredItems)
        {
            if(character.TopEmployer.Belogings.Find(x=>x.name == reqItem.name) == null)
            {
                reason = new FailReason("Requires Item - " + reqItem.name);
                return false;
            }
        }

        return true;
    }
}
