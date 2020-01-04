using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DebugAction", menuName = "DataObjects/AgentActions/DebugAction", order = 2)]
public class DebugAction : AgentAction
{
    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            return;
        }

        if (!RollSucceed(character))
        {
            if (FailureResult != null)
            {
                FailureResult.Execute(requester, character, target);
            }

            return;
        }

        if (CORE.Instance.DEBUG)
        {
            Debug.Log(this.name + " EXECUTED!");
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        return true;
    }
}
