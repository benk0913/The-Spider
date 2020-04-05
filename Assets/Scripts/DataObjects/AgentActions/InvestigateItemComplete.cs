using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "InvestigateItemComplete", menuName = "DataObjects/AgentActions/Item/InvestigateItemComplete", order = 2)]
public class InvestigateItemComplete : AgentAction
{

    public Item ItemToRemove;
    public Item replacementItem;
   
    public override void Execute(Character requester, Character character, AgentInteractable target)
    {

        Item invItem = CORE.PC.Belogings.Find(x => x.name == ItemToRemove.name);

        if (invItem == null)
        {
            return;
        }

        base.Execute(requester, character, target);

        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            return;
        }


        CORE.PC.Belogings.Remove(invItem);

        CORE.PC.Belogings.Add(replacementItem);
        InventoryPanelUI.Instance.RefreshInventory();
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        reason = null;
        return true;
    }
}
