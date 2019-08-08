using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "FireAgent", menuName = "DataObjects/PlayerActions/FireAgent", order = 2)]
public class FireAgent : PlayerAction
{
    public override void Execute(AgentInteractable target)
    {
        if (!CanDoAction(target))
        {
            GlobalMessagePrompterUI.Instance.Show("This agent is not yours to fire.", 1f, Color.yellow);
            return;
        }

        Character character = ((PortraitUI)target).CurrentCharacter;

        if(character.Employer == CORE.PC)
        {
            character.StopWorkingFor(character.WorkLocation);
        }
        else
        {
            character.StopWorkingFor(character.WorkLocation);
        }
    }

    public override bool CanDoAction(AgentInteractable target)
    {
        PortraitUI portrait = ((PortraitUI)target);

        if (portrait.CurrentCharacter == null)
        {
            return false;
        }

        if (portrait.CurrentCharacter == CORE.PC)
        {
            return false;
        }

        if (portrait.CurrentCharacter.TopEmployer != CORE.PC)
        {
            return false;
        }

        return true;
    }
}
