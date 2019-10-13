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
        string reason;
        if (!CanDoAction(requester, target, out reason))
        {
            GlobalMessagePrompterUI.Instance.Show("You cannot do that.", 1f, Color.yellow);
            return;
        }

        if (SelectAgent)
        {
            SelectAgentWindowUI.Instance.Show(
                (Character character) => { ActionToForce.Execute(requester, character, target); }
                , (Character charInQuestion) => { return charInQuestion.TopEmployer == CORE.PC && charInQuestion != CORE.PC; });
        }
        else
        {
            ActionToForce.Execute(requester, CORE.PC, target);
        }
    }

    public override bool CanDoAction(Character requester, AgentInteractable target, out string reason)
    {
        reason = "";

        if (!ActionToForce.CanDoAction(requester, CORE.PC, target, out reason))
        {
            return false;
        }
   
        return true;
    }
}
