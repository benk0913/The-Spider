using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SearchRuins", menuName = "DataObjects/AgentActions/Item/SearchRuins", order = 2)]
public class SearchRuins : AgentAction
{
    public DialogPiece Dialog;

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            return;
        }

        GameClock.Instance.PassTime();

        Dictionary<string, object> parameters = new Dictionary<string, object>();

        parameters.Add("Actor", CORE.PC);
        parameters.Add("NoGender", true);
        parameters.Add("ActorName", CORE.PC.name);
        parameters.Add("Location", CORE.PC.CurrentLocation);
        parameters.Add("LocationName", CORE.PC.CurrentLocation.Name);
        parameters.Add("Target", null);
        parameters.Add("TargetName", "");

        DialogWindowUI.Instance.StartNewDialog(Dialog, parameters);
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

        if (item.CurrentItem.ConsumeActions.Find(x=>x.name == this.name) == null)
        {
            return false;
        }

        return true;
    }
}
