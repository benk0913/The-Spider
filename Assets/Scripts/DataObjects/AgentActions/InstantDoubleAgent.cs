using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "InstantDoubleAgent", menuName = "DataObjects/AgentActions/InstantDoubleAgent", order = 2)]
public class InstantDoubleAgent : AgentAction //DO NOT INHERIT FROM
{
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

        character.PupperTurnsLeft = 35;
        character.PuppetOf = CORE.PC.CurrentFaction;

        if(CORE.Instance.Factions.Find(X=>X.name == "Constabulary").FactionHead.name == character.name) //If head constable
        {
            if(character.PuppetOf.FactionHead == CORE.PC)
            {
                WarningWindowUI.Instance.Show("Your heat has been cleared.", null, false, null);
                CORE.PC.Heat = 0;
            }
        }
        
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        reason = null;
        if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
        {
            Character targetChar = ((PortraitUI)target).CurrentCharacter;

            if (targetChar != null)
            {
                if (targetChar.TopEmployer == targetChar)
                {
                    reason = new FailReason(targetChar.name + " is not stupid. (Will not betray self)");
                    return false;
                }
            }
        }
        return true;
    }
}
