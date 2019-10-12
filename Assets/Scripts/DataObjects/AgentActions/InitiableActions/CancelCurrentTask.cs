using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CancelCurrentTask", menuName = "DataObjects/AgentActions/CancelCurrentTask", order = 2)]
public class CancelCurrentTask : AgentAction
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        string reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            return;
        }

        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        targetChar.StopDoingCurrentTask();
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out string reason)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if(targetChar.CurrentTaskEntity == null)
        {
            return false;
        }

        if (!targetChar.CurrentTaskEntity.CurrentTask.Cancelable)
        {
            return false;
        }

        return true;
    }
}
