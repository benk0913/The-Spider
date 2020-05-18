using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ForgeryOnPerson", menuName = "DataObjects/AgentActions/ForgeryOnPerson", order = 2)]
public class ForgeryOnPerson : AgentAction //DO NOT INHERIT FROM
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        ForgeryWindowUI.Instance.Show(targetChar);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        reason = null;

        if (target.GetType() != typeof(PortraitUI) && target.GetType() != typeof(PortraitUIEmployee))
        {
            return false;
        }


        Character targetChar = ((PortraitUI)target).CurrentCharacter;


        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if(targetChar.TopEmployer == targetChar)
        {
            reason = new FailReason("Unavailable With This Character");
            return false;
        }

        if (targetChar.CurrentFaction.name == "House Howund")
        {
            reason = new FailReason("Your proposal goes against the lord's interests.");
            return false;
        }
        
        if (!targetChar.IsKnown("Name", requester))
        {
            reason = new FailReason("You don't know the targets name.");
            return false;
        }

        if (!targetChar.IsKnown("HomeLocation", requester))
        {
            reason = new FailReason("You don't know where the target lives.");
            return false;
        }

        return true;
    }
}
