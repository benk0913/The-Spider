using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GetLiberated", menuName = "DataObjects/AgentActions/GetLiberated", order = 2)]
public class GetLiberated : AgentAction //DO NOT INHERIT FROM
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
                CORE.Instance.SplineAnimationObject("GoodReputationCollectedWorld",
                  character.CurrentLocation.transform,
                  StatsViewUI.Instance.transform,
                  null,
                  false);
        }

        character.Reputation += 1;
        character.TopEmployer.Reputation += 1;

        if (character.TopEmployer == CORE.PC)
        {
            TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance("You liberate your minions - Reputation +1",
        ResourcesLoader.Instance.GetSprite("pointing"),
        CORE.PC));
        }

        if (target.GetType() == typeof(LocationEntity))
        {
            LocationEntity targetLocation = (LocationEntity)target;

            if (this.Task != null)
            {
                CORE.Instance.GenerateLongTermTask(this.Task, requester, character, targetLocation, null, -1, null, this);
            }

            character.ExitPrison();
        }
        else if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
        {
            PortraitUI targetCharacter = (PortraitUI)target;
            
            LocationEntity targetLocation  = CORE.Instance.GetClosestLocationWithTrait(CORE.Instance.Database.LawAreaTrait, targetCharacter.CurrentCharacter.CurrentLocation);

            if (this.Task != null)
            {
                CORE.Instance.GenerateLongTermTask(this.Task, requester, character, targetLocation, null, -1, null, this);
            }

            character.ExitPrison();
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
