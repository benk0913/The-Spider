using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationPortraitUI : MonoBehaviour
{
    [SerializeField]
    Image Icon;

    [SerializeField]
    Image Frame;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    LocationEntity CurrentLocation;

    public void SetLocation(LocationEntity location)
    {
        CurrentLocation = location;

        if(CurrentLocation == null)
        {
            Icon.color = Color.black;
            Frame.color = CORE.Instance.Database.DefaultFaction.FactionColor;
            return;
        }

        Icon.sprite = CurrentLocation.CurrentProperty.Icon;

        if (CurrentLocation.OwnerCharacter != null)
        {
            Frame.color = CurrentLocation.OwnerCharacter.CurrentFaction.FactionColor;
        }
        else
        {
            Frame.color = CORE.Instance.Database.DefaultFaction.FactionColor;
        }

        TooltipTarget.Text = CurrentLocation.CurrentProperty.name + " - Which belongs to " + CurrentLocation.OwnerCharacter.name;
    }
}
