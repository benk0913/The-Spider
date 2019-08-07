using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "InviteAgent", menuName = "DataObjects/PlayerActions/InviteAgent", order = 2)]
public class InviteAgent : PlayerAction
{
    public override void Execute(AgentInteractable target)
    {
        if (!CanDoAction(target))
        {
            GlobalMessagePrompterUI.Instance.Show("This agent is uninvitable.", 1f, Color.yellow);
            return;
        }

        Character character = ((PortraitUI)target).CurrentCharacter;

        character.StopWorkingFor(character.WorkLocation);
        character.StartWorkingFor(CORE.PC.PropertiesOwned[0]);
    }

    public override bool CanDoAction(AgentInteractable target)
    {
        PortraitUI portrait = ((PortraitUI)target);

        if(portrait.CurrentCharacter == null)
        {
            return false;
        }

        if (portrait.CurrentCharacter == CORE.PC)
        {
            return false;
        }

        if (portrait.CurrentCharacter.Employer == CORE.PC)
        {
            return false;
        }

        if (portrait.CurrentCharacter.TopEmployer != CORE.PC)
        {
            return false;
        }


        //TODO Better way to find the manor?
        if (CORE.PC.PropertiesOwned[0].EmployeesCharacters.Count 
            >= 
            CORE.PC.PropertiesOwned[0].CurrentProperty.PropertyLevels[CORE.PC.PropertiesOwned[0].Level-1].MaxEmployees) 
        {
            return false;
        }

        return true;
    }
}
