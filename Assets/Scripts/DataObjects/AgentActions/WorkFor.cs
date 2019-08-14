﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WorkFor", menuName = "DataObjects/AgentActions/WorkFor", order = 2)]
public class WorkFor : AgentAction
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        if (!CanDoAction(requester, character, target))
        {
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

        character.StopWorkingFor(character.WorkLocation);
        character.StartWorkingFor(targetLocation);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target)
    {
        LocationEntity location = (LocationEntity)target;

        if (!base.CanDoAction(requester, character, target))
        {
            return false;
        }

        if (character.TopEmployer != requester)
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
