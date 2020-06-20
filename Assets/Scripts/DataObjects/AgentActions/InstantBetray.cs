using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "InstantBetray", menuName = "DataObjects/AgentActions/InstantBetray", order = 2)]
public class InstantBetray : AgentAction //DO NOT INHERIT FROM
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

        character.BetrayEmployer();
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        LocationEntity targetEntity = (LocationEntity)target;

        reason = null;

        return true;
    }
}
