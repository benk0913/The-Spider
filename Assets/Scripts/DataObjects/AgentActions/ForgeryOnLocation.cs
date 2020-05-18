using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ForgeryOnLocation", menuName = "DataObjects/AgentActions/ForgeryOnLocation", order = 2)]
public class ForgeryOnLocation : AgentAction //DO NOT INHERIT FROM
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        LocationEntity targetLocation = (LocationEntity)target;

        ForgeryWindowUI.Instance.Show(targetLocation);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        reason = null;

        if(target.GetType() != typeof(LocationEntity))
        {
            return false;
        }

        LocationEntity targetLocation = (LocationEntity)target;


        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        List<Character> agents = requester.CharactersInCommand.FindAll(x => x.IsAgent);
        if (agents == null || agents.Count == 0)
        {
            reason = new FailReason("No available agents to own this location.");
            return false;
        }

        if (targetLocation.OwnerCharacter == null)
        {
            reason = new FailReason("This location is not owned by anyone.");
            return false;
        }

        if(targetLocation.OwnerCharacter.TopEmployer == targetLocation.OwnerCharacter)
        {
            reason = new FailReason("Unavailable With This Character");
            return false;
        }

        if (targetLocation.OwnerCharacter.CurrentFaction.name == "House Howund")
        {
            reason = new FailReason("Your proposal goes against the lord's interests.");
            return false;
        }

        if (!targetLocation.Known.IsKnown("Existance", requester))
        {
            reason = new FailReason("You don't know where this location is.");
            return false;
        }

        if (!targetLocation.OwnerCharacter.IsKnown("Name", requester))
        {
            reason = new FailReason("You don't know the owners name.");
            return false;
        }

        if (!targetLocation.OwnerCharacter.IsKnown("HomeLocation", requester))
        {
            reason = new FailReason("You don't know where the owner lives.");
            return false;
        }

        return true;
    }
}
