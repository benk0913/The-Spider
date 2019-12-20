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
        reason = null;

        if (!base.CanDoAction(requester,target,out reason))
        {
            return false;
        }

        LocationEntity location = (LocationEntity)target;

        if (location.OwnerCharacter == null || location.OwnerCharacter.TopEmployer != requester)
        {
            return false;
        }

        if (location.CurrentProperty.PlotType == CORE.Instance.Database.UniquePlotType)
        {
            return false;
        }

        return true;
    }
}
