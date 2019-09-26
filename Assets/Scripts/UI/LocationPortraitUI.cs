using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
            TooltipTarget.Text = "Unknown...";
            return;
        }
        else
        {
            Icon.color = Color.white;

            if (CurrentLocation.OwnerCharacter != null)
            {
                Frame.color = CurrentLocation.OwnerCharacter.CurrentFaction.FactionColor;
            }
            else
            {
                Frame.color = CORE.Instance.Database.DefaultFaction.FactionColor;
            }

            Icon.sprite = CurrentLocation.CurrentProperty.Icon;

            TooltipTarget.Text = CurrentLocation.CurrentProperty.name + " - Which belongs to "
                + (CurrentLocation.OwnerCharacter == null ?
                " no one."
                :
                CurrentLocation.OwnerCharacter.name);
        }
    }

    public void ShowLocationInfo()
    {
        LocationInfoUI.Instance.Show(CurrentLocation);
    }
}
