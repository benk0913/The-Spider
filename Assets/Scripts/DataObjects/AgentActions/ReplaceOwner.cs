using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReplaceOwner", menuName = "DataObjects/AgentActions/ReplaceOwner", order = 2)]
public class ReplaceOwner : AgentAction
{

    public override void Execute(Character character, AgentInteractable target)
    {
        base.Execute(character, target);

        if (!CanDoAction(character, target))
        {
            return;
        }

        LocationEntity location = (LocationEntity)target;

        if(character.WorkLocation == location && character.Employer != CORE.PC)
        {
            WarningWindowUI.Instance.Show("Warning! Doing this will cut off the chain of command you have on this agent: \n <color=red> * this means it will no longer work for you. \n * All of it's properties will be out of your control. </color>", delegate 
            {
                DoTheReplacement(character, location);
            });

            return;
        }

        DoTheReplacement(character, location);
    }

    void DoTheReplacement(Character character, LocationEntity location)
    {
        if (location == character.WorkLocation)
        {
            character.StopWorkingFor(location);
        }

        character.StartOwningLocation(location);
    }

    public override bool CanDoAction(Character character, AgentInteractable target)
    {
        LocationEntity location = (LocationEntity)target;

        if(character.TopEmployer != CORE.PC)
        {
            return false;   
        }

        if(location.OwnerCharacter == CORE.PC)
        {
            return false;
        }

        if(location.OwnerCharacter.TopEmployer != CORE.PC)
        {
            return false;
        }

        if(character.PropertiesOwned.Contains(location))
        {
            return false;
        }

        return true;
    }
}
