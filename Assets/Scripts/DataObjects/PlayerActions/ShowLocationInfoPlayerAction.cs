using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ShowLocationInfoPlayerAction", menuName = "DataObjects/PlayerActions/ShowLocationInfoPlayerAction", order = 2)]
public class ShowLocationInfoPlayerAction : PlayerAction
{
    public override void Execute(Character requester, AgentInteractable target)
    {
        FailReason reason;
        if (!CanDoAction(requester, target, out reason))
        {
            GlobalMessagePrompterUI.Instance.Show("This location is not yours to select.", 1f, Color.yellow);
            return;
        }

        LocationEntity location = (LocationEntity)target;

        LocationInfoUI.Instance.Show(location);
    }

    public override bool CanDoAction(Character requester, AgentInteractable target, out FailReason reason)
    {
        reason = null;
        
        if (!((LocationEntity)target).Known.GetKnowledgeInstance("Existance").IsKnownByCharacter(requester))
        {
            //reason = new FailReason("This location is not known to you.");
            return false;
        }

        return true;
    }
}
