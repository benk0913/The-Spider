﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AttemptGatheringRumorsAboutPerson", menuName = "DataObjects/AgentActions/AttemptGatheringRumorsAboutPerson", order = 2)]
public class AttemptGatheringRumorsAboutPerson : AgentAction //DO NOT INHERIT FROM
{
    public LongTermTask Task;

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            GlobalMessagePrompterUI.Instance.Show(reason.Key, 1f, Color.red);
            return;
        }
        
        if (!RollSucceed(character))
        {
            if (FailureResult != null)
            {
                FailureResult.Execute(requester, character, target);
            }

            return;
        }

        if(Task == null)
        {
            return;
        }

        if (target.GetType() == typeof(LocationEntity))
        {
            CORE.Instance.GenerateLongTermTask(this.Task, requester, character, character.WorkLocation);
        }
        else
        {
            Character targetChar = ((PortraitUI)target).CurrentCharacter;
            CORE.Instance.GenerateLongTermTask(this.Task, requester, character, character.WorkLocation, targetChar);
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;
        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (requester != character && requester != CORE.Instance.Database.GOD && character.TopEmployer != requester)
        {
            return false;
        }

        if(targetChar.TopEmployer == requester)
        {
            return false;
        }

        if(targetChar.IsKnown("CurrentLocation", character.TopEmployer))
        {
            return false;
        }

        if (!targetChar.IsKnown("Appearance", character.TopEmployer) && !targetChar.IsKnown("Name", character.TopEmployer) && !targetChar.IsKnown("WorkLocation", character.TopEmployer))
        {
            reason = new FailReason("You don't know either the NAME, WORK LOCATION nor the LOOKS of this perosn.");
            return false;
        }

        return true;
    }
}
