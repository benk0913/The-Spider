using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ItemBeSentToPlayer", menuName = "DataObjects/AgentActions/Item/ItemBeSentToPlayer", order = 2)]
public class ItemBeSentToPlayer : AgentAction
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

        GameObject itemObj = Instantiate(itemUI.CurrentItem.RealWorldPrefab);

        itemObj.transform.position = LetterDispenserEntity.Instance.transform.position + new Vector3(0f, 0.1f, 0f);

        RoomsManager.Instance.AddItem(itemObj);

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

        if(item.CurrentItem.RealWorldPrefab == null)
        {
            return false;
        }

        return true;
    }
}
