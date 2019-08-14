using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "FireAgent", menuName = "DataObjects/PlayerActions/FireAgent", order = 2)]
public class FireAgent : PlayerAction
{
    public override void Execute(Character requester, AgentInteractable target)
    {
        if (!CanDoAction(requester, target))
        {
            GlobalMessagePrompterUI.Instance.Show("This agent is not yours to fire.", 1f, Color.yellow);
            return;
        }

        Character character = ((PortraitUI)target).CurrentCharacter;

        if (character.WorkLocation.OwnerCharacter.GetRelationsWith(character) > 5)
        {
            character.WorkLocation.OwnerCharacter.DynamicRelationsModifiers.Add
            (
            new DynamicRelationsModifier(
            new RelationsModifier("Took an employee I liked!", -2)
            , 10
            , requester)
            );
        }

        if (character.Employer == requester)
        {
            character.StopWorkingFor(character.WorkLocation);
        }
        else
        {
            character.StopWorkingFor(character.WorkLocation);
        }
    }

    public override bool CanDoAction(Character requester, AgentInteractable target)
    {
        PortraitUI portrait = ((PortraitUI)target);

        if (portrait.CurrentCharacter == null)
        {
            return false;
        }

        if (portrait.CurrentCharacter == requester)
        {
            return false;
        }

        if (portrait.CurrentCharacter.TopEmployer != requester)
        {
            return false;
        }

        return true;
    }
}
