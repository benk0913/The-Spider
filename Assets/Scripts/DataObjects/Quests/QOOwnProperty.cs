using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOOwnProperty", menuName = "DataObjects/Quests/QuestObjectives/QOOwnProperty", order = 2)]
public class QOOwnProperty : QuestObjective
{
    public Property TargetProperty;

    LocationEntity PropertyFound;

    public override bool Validate()
    {
        return CharacterHasProperty(CORE.PC, TargetProperty);
    }

    public bool CharacterHasProperty(Character character, Property targetProperty)
    {
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