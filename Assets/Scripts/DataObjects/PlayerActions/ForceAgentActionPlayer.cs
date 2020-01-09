using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ForceAgentActionPlayer", menuName = "DataObjects/PlayerActions/ForceAgentActionPlayer", order = 2)]
public class ForceAgentActionPlayer : PlayerAction
{
    [SerializeField]
    AgentAction ActionToForce;

    [SerializeField]
    public bool SelectAgent = true;

    public override void Execute(Character requester, AgentInteractable target)
    {
        FailReason reason;
        if (!CanDoAction(requester, target, out reason))
        {
            GlobalMessagePrompterUI.Instance.Show("You cannot do that."+reason?.Key, 2f, Color.yellow);
            return;
        }

        if (SelectAgent)
        {
            SelectAgentWindowUI.Instance.Show(
                (Character character) => { ActionToForce.Execute(requester, character, target); SelectedPanelUI.Instance.Deselect(); }
                , x => { return x.TopEmployer == CORE.PC && x.TopEmployer != x && x.IsAgent; });
        }
        else
        {
            ActionToForce.Execute(requester, requester, target);
        }
    }

    public override bool CanDoAction(Character requester, AgentInteractable target, out FailReason reason)
    {
        reason = null;

        if (!ActionToForce.CanDoAction(requester, requester, target, out reason))
        {
            return false;
        }

   
        return true;
    }
}
