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

        character.Reputation += 1;
        character.TopEmployer.Reputation += 1;

        if (target.GetType() == typeof(LocationEntity))
        {
            LocationEntity targetLocation = (LocationEntity)target;

            CORE.Instance.GenerateLongTermTask(this.Task, requester, character, targetLocation);

            character.ExitPrison();
        }
        else if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
        {
            PortraitUI targetCharacter = (PortraitUI)target;
            
            LocationEntity targetLocation  = CORE.Instance.GetClosestLocationWithTrait(CORE.Instance.Database.LawAreaTrait, targetCharacter.CurrentCharacter.CurrentLocation);

            CORE.Instance.GenerateLongTermTask(this.Task, requester, character, targetLocation);
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
