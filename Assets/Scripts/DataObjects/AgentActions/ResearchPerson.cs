using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ResearchPerson", menuName = "DataObjects/AgentActions/ResearchPerson", order = 2)]
public class ResearchPerson : AgentAction //DO NOT INHERIT FROM
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        ResearchCharacterWindowUI.Instance.Show(targetChar);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        reason = null;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
        {
            if (((PortraitUI)target).CurrentCharacter.TopEmployer == CORE.PC)
            {
                return false;
            }
        }

        return true;
    }
}
