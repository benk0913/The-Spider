using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WorkFor", menuName = "DataObjects/AgentActions/WorkFor", order = 2)]
public class WorkFor : AgentAction
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

        LocationEntity targetLocation = (LocationEntity)target;

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

        character.StopWorkingForCurrentLocation();
        character.StartWorkingFor(targetLocation);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        LocationEntity location = (LocationEntity)target;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (character.TopEmployer != requester)
        {
            return false;
        }

        if(location.EmployeesCharacters.Count > 0 && location.EmployeesCharacters.Count >= location.CurrentProperty.PropertyLevels[location.Level-1].MaxEmployees) // (Has free slots?)
        {
            reason = new FailReason(location.CurrentProperty.name + " is already full of employees.");
            return false;
        }

        if(location.CurrentProperty.MinAge > character.Age)
        {
            reason = new FailReason(character.name + " is too young...");
            return false;
        }

        if (location == character.WorkLocation)
        {
            return false;
        }

        if (character.PropertiesOwned.Contains(location))
        {
            reason = new FailReason(character.name + " owns the place.");
            return false;
        }

        Character tempChar = location.OwnerCharacter;
        while(tempChar != null)
        {
            if(tempChar == character)
            {
                reason = new FailReason(character.name+" is an employer of "+location.OwnerCharacter.name);
                return false;
            }

            tempChar = tempChar.Employer;
        }

        return true;
    }
}
