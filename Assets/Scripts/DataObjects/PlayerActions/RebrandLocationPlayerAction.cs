using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RebrandLocationPlayerAction", menuName = "DataObjects/PlayerActions/RebrandLocationPlayerAction", order = 2)]
public class RebrandLocationPlayerAction : PlayerAction
{
    public override void Execute(Character requester, AgentInteractable target)
    {
        FailReason reason;
        if (!CanDoAction(requester, target, out reason))
        {
            GlobalMessagePrompterUI.Instance.Show("You cannot change this property.", 1f, Color.yellow);
            return;
        }

        LocationEntity location = (LocationEntity)target;

        RebrandWindowUI.Instance.Show(location);
    }

    public override bool CanDoAction(Character requester, AgentInteractable target, out FailReason reason)
    {
        LocationEntity location = (LocationEntity)target;

        reason = null;

        if (location.OwnerCharacter == null || location.OwnerCharacter.TopEmployer != requester)
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
