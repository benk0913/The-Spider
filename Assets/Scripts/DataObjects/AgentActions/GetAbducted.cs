using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GetAbducted", menuName = "DataObjects/AgentActions/GetAbducted", order = 2)]
public class GetAbducted : AgentAction //DO NOT INHERIT FROM
{
    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        //FailReason reason;
        //if (!CanDoAction(requester, character, target, out reason))
        //{
        //    return;
        //}
        
        if (!RollSucceed(character))
        {
            if (FailureResult != null)
            {
                FailureResult.Execute(requester, character, target);
            }

            return;
        }

        base.Execute(requester, character, target);

        
        if (character.TopEmployer == CORE.PC)
        {
            CORE.Instance.SplineAnimationObject("BadReputationCollectedWorld",
              character.CurrentLocation.transform,
              StatsViewUI.Instance.transform,
              null,
              false);
        }

        character.Reputation -= 1;
        character.TopEmployer.Reputation -= 1;

        if (target.GetType() == typeof(LocationEntity))
        {
            LocationEntity targetLocation = (LocationEntity)target;

            character.EnterPrison(targetLocation);
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
