using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AttemptScheme", menuName = "DataObjects/AgentActions/Scheme/AttemptScheme", order = 2)]
public class AttemptScheme : AgentAction //DO NOT INHERIT FROM
{
    public SchemeType Scheme;

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            return;
        }
        
        if (FailureResult != null && !RollSucceed(character))
        {
            if (FailureResult != null)
            {
                FailureResult.Execute(requester, character, target);
            }

            return;
        }

        PlottingWindowUI.Instance.Show(target, Scheme, character);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
        {
            Character targetCharacter = ((PortraitUI)target).CurrentCharacter;

            if (targetCharacter == character)
            {
                return false;
            }

            if(targetCharacter == requester)
            {
                return false;
            }

            if(targetCharacter.IsInHiding)
            {
                reason = new FailReason("The Target Is Temporarily In Hiding...");
                return false;
            }

            if(!targetCharacter.IsKnown("CurrentLocation",requester))
            {
                reason = new FailReason("Don't know the targets current location");
                return false;
            }

            if(targetCharacter.PrisonLocation != null 
                && targetCharacter.PrisonLocation.OwnerCharacter != null 
                && targetCharacter.PrisonLocation.OwnerCharacter.TopEmployer == character.TopEmployer)
            {
                reason = new FailReason("No need to plot against your own prisoners.");
                return false;
            }
        }
        else if (target.GetType() == typeof(LocationEntity))
        {
            LocationEntity targetLocation = ((LocationEntity)target);

            if(!targetLocation.Known.IsKnown("Existance", requester))
            {
                return false;
            }
        }


        return true;
    }
}
