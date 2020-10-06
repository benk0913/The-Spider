using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ShowSellPrisonerWindow", menuName = "DataObjects/PlayerActions/ShowSellPrisonerWindow", order = 2)]
public class ShowSellPrisonerWindow : PlayerAction
{
    public override void Execute(Character requester, AgentInteractable target)
    {
        FailReason reason;
        if (!CanDoAction(requester, target, out reason))
        {
            GlobalMessagePrompterUI.Instance.Show("This character is not your prisoner.", 1f, Color.yellow);
            return;
        }

        PortraitUI agent = (PortraitUI)target;

        FleshVendorUI.Instance.Show(agent.CurrentCharacter);
    }

    public override bool CanDoAction(Character requester, AgentInteractable target, out FailReason reason)
    {
        reason = null;

        Character targetCharacter;
        if(target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
        {
            targetCharacter = ((PortraitUI)target).CurrentCharacter;
        }
        else
        {
            return false;
        }

        if(targetCharacter == null)
        {
            return false;
        }

        if(targetCharacter.PrisonLocation == null)
        {
            return false;
        }

        if(targetCharacter.PrisonLocation.OwnerCharacter == null)
        {
            return false;
        }

        if (targetCharacter.PrisonLocation.OwnerCharacter.TopEmployer != CORE.PC)
        {
            return false;
        }

        return true;
    }
}
