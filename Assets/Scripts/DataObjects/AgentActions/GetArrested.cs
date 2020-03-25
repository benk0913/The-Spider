using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GetArrested", menuName = "DataObjects/AgentActions/GetArrested", order = 2)]
public class GetArrested : AgentAction //DO NOT INHERIT FROM
{
    public LongTermTask Task;

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

            if (!targetLocation.CurrentProperty.Traits.Contains(CORE.Instance.Database.LawAreaTrait)) //LOCATION IS NOT A CONSTABULARY / ETC..
            {
                targetLocation = CORE.Instance.GetClosestLocationWithTrait(CORE.Instance.Database.LawAreaTrait, targetLocation);
                if (targetLocation != null)
                {
                    target = targetLocation;
                }
            }

            CORE.Instance.GenerateLongTermTask(this.Task, requester, character, targetLocation);
            character.EnterPrison(targetLocation);
        }
        else if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
        {
            PortraitUI targetCharacter = (PortraitUI)target;
            
            LocationEntity targetLocation  = CORE.Instance.GetClosestLocationWithTrait(CORE.Instance.Database.LawAreaTrait, targetCharacter.CurrentCharacter.CurrentLocation);
            

            CORE.Instance.GenerateLongTermTask(this.Task, requester, character, targetLocation);
            character.EnterPrison(targetLocation);
        }

    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        reason = null;
        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        return true;
    }
}
