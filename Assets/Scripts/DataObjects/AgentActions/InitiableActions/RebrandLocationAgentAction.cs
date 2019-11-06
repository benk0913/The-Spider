using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RebrandLocationAgentAction", menuName = "DataObjects/AgentActions/RebrandLocationAgentAction", order = 2)]
public class RebrandLocationAgentAction : AgentAction
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
        
        LocationEntity location = (LocationEntity)target;

        RebrandWindowUI.Instance.Show(location);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        LocationEntity location = (LocationEntity)target;

        reason = null;

        if (location.OwnerCharacter == null || location.OwnerCharacter.TopEmployer != character.TopEmployer)
        {
            return false;
        }

        if (location.CurrentProperty.PlotType == CORE.Instance.Database.UniquePlotType)
        {
            reason = new FailReason("This location is unique and cannot be changed.");
            return false;
        }

        return true;
    }
}
