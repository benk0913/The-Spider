using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PurchasePlotAction", menuName = "DataObjects/AgentActions/PurchasePlotAction", order = 2)]
public class PurchasePlotAction : AgentAction
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            return;
        }

        if(!RollSucceed(character))
        {
            if(FailureResult != null)
            {
                FailureResult.Execute(requester, character, target);
            }

            return;
        }

        LocationEntity targetEntity = (LocationEntity)target;

        targetEntity.PurchasePlot(requester, character);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        reason = null;
        LocationEntity targetEntity = (LocationEntity)target;

        if(!targetEntity.Known.GetKnowledgeInstance("Existance").IsKnownByCharacter(character.TopEmployer))
        {
            //reason = new FailReason("This location is not known to you.");

            return false;
        }

        if(targetEntity.OwnerCharacter != null)
        {
            return false;
        }

        if(targetEntity.Traits.Contains(CORE.Instance.Database.PublicAreaTrait))
        {
            return false;
        }

        return true;
    }
}
