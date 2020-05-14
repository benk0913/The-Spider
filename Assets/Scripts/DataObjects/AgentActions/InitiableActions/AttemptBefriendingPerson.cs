using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AttemptBefriendingPerson", menuName = "DataObjects/AgentActions/AttemptBefriendingPerson", order = 2)]
public class AttemptBefriendingPerson : AgentAction //DO NOT INHERIT FROM
{
    public LongTermTask Task;

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

        if(Task == null)
        {
            return;
        }

        if (target.GetType() == typeof(LocationEntity))
        {
            CORE.Instance.GenerateLongTermTask(this.Task, requester, character, (LocationEntity)target, null, -1, null, this);
        }
        else if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
        {
            Character targetChar = ((PortraitUI)target).CurrentCharacter;
            CORE.Instance.GenerateLongTermTask(this.Task, requester, character, targetChar.CurrentLocation, targetChar, -1, null, this);
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;
        reason = null;

        if (requester != character && requester != CORE.Instance.Database.GOD && character.TopEmployer != requester)
        {
            return false;
        }

        if (targetChar == character)
        {
            return false;
        }

        if (targetChar.TopEmployer == character.TopEmployer)
        {
            return false;
        }

        if (!targetChar.IsKnown("CurrentLocation", character.TopEmployer))
        {
            reason = new FailReason("You don't know where this person is...");
            return false;
        }

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        return true;
    }
}
