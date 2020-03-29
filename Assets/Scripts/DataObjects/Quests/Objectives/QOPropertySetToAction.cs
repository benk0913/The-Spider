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
        if(ParentQuest.ForCharacter == null)
        {
            Debug.Log("NO FOR CHARACTER");
            return false;
        }

        return CharacterHasPropertyInAction(ParentQuest.ForCharacter, TargetProperty);
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

    public override GameObject GetMarkerTarget()
    {
        if(!SelectedPanelUI.Instance.LocationPanel.gameObject.activeInHierarchy)
        {
            return null;
        }

        if(SelectedPanelUI.Instance.LocationPanel.CurrentLocation.OwnerCharacter == null)
        {
            return null;
        }

        if (SelectedPanelUI.Instance.LocationPanel.CurrentLocation.OwnerCharacter.TopEmployer != CORE.PC)
        {
            return null;
        }

        if (SelectedPanelUI.Instance.LocationPanel.CurrentLocation.CurrentProperty.name != TargetProperty.name)
        {
            return null;
        }


        return SelectedPanelUI.Instance.LocationPanel.ActionGrid.GetChild(TargetActionIndex).gameObject;
    }

}