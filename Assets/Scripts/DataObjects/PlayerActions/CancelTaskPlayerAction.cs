using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CancelTaskPlayerAction", menuName = "DataObjects/PlayerActions/CancelTaskPlayerAction", order = 2)]
public class CancelTaskPlayerAction : PlayerAction
{

    public override void Execute(Character requester, AgentInteractable target)
    {
        FailReason reason;
        if (!CanDoAction(requester, target, out reason))
        {
            GlobalMessagePrompterUI.Instance.Show("You cannot interact with that task.", 1f, Color.yellow);
            return;
        }

        if (target.GetType() == typeof(ActionPortraitUI))
        {

            ActionPortraitUI taskUI = (ActionPortraitUI)target;

            taskUI.CurrentEntity.Cancel();
        }
        else if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
        {
            PortraitUI portrait = (PortraitUI)target;

            portrait.CurrentCharacter.CurrentTaskEntity.Cancel();
        }
    }

    public override bool CanDoAction(Character requester, AgentInteractable target, out FailReason reason)
    {
        reason = null;

        if (target.GetType() == typeof(ActionPortraitUI))
        {
            ActionPortraitUI taskUI = (ActionPortraitUI)target;

            if (taskUI.CurrentEntity.CurrentRequester != requester)
            {
                return false;
            }

            if (!taskUI.CurrentEntity.CurrentTask.Cancelable)
            {
                return false;
            }
        }
        else if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
        {
            PortraitUI portrait = (PortraitUI)target;

            if(portrait.CurrentCharacter == CORE.PC)
            {
                return false;
            }

            if(portrait.CurrentCharacter == null)
            {
                return false;
            }

            if (portrait.CurrentCharacter.TopEmployer != requester)
            {
                return false;
            }

            if (portrait.CurrentCharacter.CurrentTaskEntity == null)
            {
                return false;
            }

            if (!portrait.CurrentCharacter.CurrentTaskEntity.CurrentTask.Cancelable)
            {
                return false;
            }
            
        }
        
        return true;
    }
}
