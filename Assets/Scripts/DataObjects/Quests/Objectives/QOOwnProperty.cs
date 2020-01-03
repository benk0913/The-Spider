﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOOwnProperty", menuName = "DataObjects/Quests/QuestObjectives/QOOwnProperty", order = 2)]
public class QOOwnProperty : QuestObjective
{
    public Property TargetProperty;

    LocationEntity PropertyFound;

    public override bool Validate()
    {
        return CharacterHasProperty(ParentQuest.ForCharacter, TargetProperty);
    }

    public bool CharacterHasProperty(Character character, Property targetProperty)
    {
        if(character == null)
        {
            return false;
        }

        foreach (LocationEntity location in character.PropertiesOwned)
        {
            if (location.CurrentProperty == TargetProperty)
            {
                return true;
            }

            foreach(Character employee in location.EmployeesCharacters)
            {
                if(CharacterHasProperty(employee, targetProperty))
                {
                    return true;
                }
            }
        }

        return false;
    }
    
}