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

        if(targetEntity == null || targetEntity.Known == null)
        {
            Debug.LogError("NO TARGET!? ");
            return false;
        }

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

        if (targetEntity.OwnerCharacter.CurrentFaction.name != CORE.Instance.Database.DefaultFaction.name && targetEntity.OwnerCharacter.CurrentFaction.name != CORE.Instance.Database.NoFaction.name)
        {
            reason = new FailReason("Owner refuses every offer. (Not independant, Try bribing instead...)");
            return false;
        }

        if (CORE.PC.CGold < (targetEntity.LandValue *2))
        {
            reason = new FailReason("Not Enough Gold - ("+CORE.PC.CGold+"/"+ (targetEntity.LandValue * 2)+")");
            return false;
        }

        return true;
    }
}
