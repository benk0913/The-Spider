using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "InvestigateItem", menuName = "DataObjects/AgentActions/Item/InvestigateItem", order = 2)]
public class InvestigateItem : AgentAction
{
    public LongTermTask TaskToExecute;

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

        CORE.Instance.GenerateLongTermTask(TaskToExecute, requester, character, character.CurrentLocation, null, -1, null, this);

        Item item = CORE.PC.Belogings.Find(X => X.name == itemUI.CurrentItem.name);

        if (item != null)
        {
            CORE.PC.Belogings.Remove(item);
            InventoryPanelUI.Instance.RefreshInventory();
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        ItemUI item = (ItemUI)target;
        reason = null;

        if (!requester.Belogings.Contains(item.CurrentItem))
        {
            return false;
        }

        if (item.CurrentItem.ConsumeActions.Find(x => x.name == this.name) == null)
        {
            return false;
        }

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        return true;
    }
}
