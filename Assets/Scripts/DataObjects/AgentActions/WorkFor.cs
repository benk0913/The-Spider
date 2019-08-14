using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WorkFor", menuName = "DataObjects/AgentActions/WorkFor", order = 2)]
public class WorkFor : AgentAction
{

    public override void Execute(Character character, AgentInteractable target)
    {
        base.Execute(character, target);

        if (!CanDoAction(character, target))
        {
            return;
        }

        LocationEntity targetLocation = (LocationEntity)target;

        character.StopWorkingFor(character.WorkLocation);
        character.StartWorkingFor(targetLocation);
    }

    public override bool CanDoAction(Character character, AgentInteractable target)
    {
        LocationEntity location = (LocationEntity)target;

        if (!base.CanDoAction(character, target))
        {
            return false;
        }

        if (character.TopEmployer != CORE.PC)
        {
            return false;
        }

        if (location.OwnerCharacter.TopEmployer != CORE.PC)
        {
            return false;
        }

        if(location.EmployeesCharacters.Count >= location.CurrentProperty.PropertyLevels[location.Level-1].MaxEmployees) // (Has free slots?)
        {
            return false;
        }

        if(location.CurrentProperty.MinAge > character.Age)
        {
            return false;
        }

        if (location == character.WorkLocation)
        {
            return false;
        }

        if (character.PropertiesOwned.Contains(location))
        {
            return false;
        }

        return true;
    }
}
