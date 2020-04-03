﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AttemptSchemeAbduct", menuName = "DataObjects/AgentActions/Scheme/AttemptSchemeAbduct", order = 2)]
public class AttemptSchemeAbduct : AgentAction //DO NOT INHERIT FROM
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

            if(targetCharacter.TopEmployer == CORE.PC)
            {
                return false;
            }

            if(!targetCharacter.IsKnown("CurrentLocation",requester))
            {
                reason = new FailReason("Don't know the targets current location");
                return false;
            }

            if(targetCharacter.PrisonLocation != null)
            {
                reason = new FailReason("This character is already imprisoned somewhere.");
                return false;
            }

            List<LocationEntity> propertiesOwned = character.TopEmployer.PropertiesOwned;

            if(propertiesOwned.Find(x => x.HasFreePrisonCell) == null)
            {
                reason = new FailReason("No prison cell available to keep the target character.");
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
