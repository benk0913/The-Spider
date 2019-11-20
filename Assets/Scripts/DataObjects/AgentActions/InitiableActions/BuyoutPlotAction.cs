using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuyoutPlotAction", menuName = "DataObjects/AgentActions/BuyoutPlotAction", order = 2)]
public class BuyoutPlotAction : AgentAction
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

        targetEntity.BuyoutPlot(requester, character);
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

        if(targetEntity.OwnerCharacter == null)
        {
            return false;
        }

        if(targetEntity.CurrentProperty.PlotType == CORE.Instance.Database.UniquePlotType)
        {
            return false;
        }

        if (targetEntity.OwnerCharacter.TopEmployer == character.TopEmployer)
        {
            return false;
        }

        if (targetEntity.OwnerCharacter.CurrentFaction != CORE.Instance.Database.DefaultFaction)
        {
            reason = new FailReason("Owner refuses every offer");
            return false;
        }

        if (CORE.PC.Gold < (targetEntity.LandValue *2))
        {
            reason = new FailReason("Not Enough Gold");
            return false;
        }

        return true;
    }
}
