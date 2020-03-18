using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOPropertySelected", menuName = "DataObjects/Quests/QuestObjectives/QOPropertySelected", order = 2)]
public class QOPropertySelected : QuestObjective
{
    public Property TargetProperty;

    LocationEntity PropertyFound;

    public bool IsPlayers = false;


    public override bool Validate()
    {
        return PropertyIsSelected(ParentQuest.ForCharacter, TargetProperty);
    }

    public bool PropertyIsSelected(Character character, Property targetProperty)
    {

        if (SelectedPanelUI.Instance.LocationPanel.CurrentLocation != null
            && SelectedPanelUI.Instance.LocationPanel.CurrentLocation.CurrentProperty == TargetProperty)
        {
            if (SelectedPanelUI.Instance.LocationPanel.CurrentLocation.OwnerCharacter == null)
            {
                return false;
            }

            if (SelectedPanelUI.Instance.LocationPanel.CurrentLocation.OwnerCharacter.TopEmployer != CORE.PC)
            {
                return false;
            }

            return true;
        }

        return false;
    }


    public override GameObject GetMarkerTarget()
    {
        LocationEntity targetLocation;

        if(IsPlayers)
        {
            targetLocation = CORE.Instance.Locations.Find(x => x.CurrentProperty.name == TargetProperty.name 
            && x.OwnerCharacter != null 
            && x.OwnerCharacter.TopEmployer == CORE.PC);
        }
        else
        {
            targetLocation = CORE.Instance.Locations.Find(x => x.CurrentProperty.name == TargetProperty.name);
        }

        if (targetLocation == null)
        {
            return null;
        }

        return targetLocation.gameObject;
    }
}
