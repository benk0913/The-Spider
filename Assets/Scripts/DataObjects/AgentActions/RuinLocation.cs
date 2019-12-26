using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RuinLocation", menuName = "DataObjects/AgentActions/RuinLocation", order = 2)]
public class RuinLocation : AgentAction //DO NOT INHERIT FROM
{
    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
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

        base.Execute(requester, character, target);

        if (target.GetType() == typeof(LocationEntity))
        {
            LocationEntity targetLocation = (LocationEntity)target;
            targetLocation.BecomeRuins();
        }

    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        return true;
    }
}
