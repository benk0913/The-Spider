﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AttemptSchemeRaid", menuName = "DataObjects/AgentActions/Scheme/AttemptSchemeRaid", order = 2)]
public class AttemptSchemeRaid : AgentAction //DO NOT INHERIT FROM
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

        if (target.GetType() == typeof(PortraitUI))
        {
            Character targetCharacter = ((PortraitUI)target).CurrentCharacter;

            if (target == character)
            {
                return false;
            }

            if(!targetCharacter.IsKnown("CurrentLocation",requester))
            {
                reason = new FailReason("Don't know the targets current location");
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

            if (targetLocation.OwnerCharacter == null)
            {
                return false;
            }

            if (targetLocation.Inventory.Count == 0)
            {
                reason = new FailReason("Nothing to steal here...");
                return false;
            }
        }


        return true;
    }
}
