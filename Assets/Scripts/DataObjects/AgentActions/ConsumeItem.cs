using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ConsumeItem", menuName = "DataObjects/AgentActions/Item/ConsumeItem", order = 2)]
public class ConsumeItem : AgentAction
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

        PortraitUI portrait = ResourcesLoader.Instance.GetRecycledObject("PortraitUI").GetComponent<PortraitUI>();
        portrait.transform.position = new Vector3(9999, 9999, 9999);
        portrait.SetCharacter(character);

        foreach (AgentAction action in itemUI.CurrentItem.ConsumeActions)
        {
            action.Execute(requester, character, portrait);
        }

        portrait.gameObject.SetActive(false);

        requester.Belogings.Remove(itemUI.CurrentItem);

        InventoryPanelUI.Instance.RefreshInventory();
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        ItemUI item = (ItemUI)target;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if(!requester.Belogings.Contains(item.CurrentItem))
        {
            return false;
        }

        if (item.CurrentItem.ConsumeActions.Count == 0)
        {
            return false;
        }

        if(!item.CurrentItem.Usable)
        {
            return false;
        }

        

        return true;
    }
}
