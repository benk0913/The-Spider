using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOPropertySetToAction", menuName = "DataObjects/Quests/QuestObjectives/QOPropertySetToAction", order = 2)]
public class QOPropertySetToAction : QuestObjective
{
    public Property TargetProperty;
    public int TargetActionIndex;

    LocationEntity PropertyFound;

    public override bool Validate()
    {
        return CharacterHasPropertyInAction(CORE.PC, TargetProperty);
    }

    public bool CharacterHasPropertyInAction(Character character, Property targetProperty)
    {
        foreach (LocationEntity location in character.PropertiesOwned)
        {
            if (location.CurrentProperty == TargetProperty 
                && location.CurrentProperty.Actions.IndexOf(location.CurrentAction) == TargetActionIndex)
            {
                return true;
            }

            foreach(Character employee in location.EmployeesCharacters)
            {
                if(CharacterHasPropertyInAction(employee, targetProperty))
                {
                    return true;
                }
            }
        }

        return false;
    }
    
}