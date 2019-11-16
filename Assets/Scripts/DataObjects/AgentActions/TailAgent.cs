using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TailAgent", menuName = "DataObjects/AgentActions/Spying/TailAgent", order = 2)]
public class TailAgent : AgentAction //DO NOT INHERIT FROM
{
    [SerializeField]
    LongTermTask Task;

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        if(Task == null)
        {
            return;
        }

        Character targetChar = ((PortraitUI)target).CurrentCharacter;
        CORE.Instance.GenerateLongTermTask(this.Task, requester, character, targetChar.CurrentLocation, targetChar);

        targetChar.Known.Know("CurrentLocation", character.TopEmployer);

    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (requester != character && requester != CORE.Instance.Database.GOD && character.TopEmployer != requester)
        {
            return false;
        }

        if(targetChar == character)
        {
            return false;
        }

        if(targetChar.TopEmployer == character.TopEmployer)
        {
            return false;
        }

        if(!targetChar.IsKnown("CurrentLocation", character.TopEmployer))
        {
            reason = new FailReason("You don't know where this character is...");
            return false;
        }

        if (!targetChar.IsKnown("Appearance", character.TopEmployer))
        {
            reason = new FailReason("You don't know how this character looks like...");
            return false;
        }

        return true;
    }
}
