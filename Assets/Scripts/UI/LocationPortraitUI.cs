using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LocationPortraitUI : MonoBehaviour, IPointerClickHandler
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
            TooltipTarget.SetTooltip("Unknown...");
            return;
        }
        else
        {
            Icon.color = Color.white;

            if (CurrentLocation.OwnerCharacter != null)
            {
                if (CurrentLocation.OwnerCharacter.IsKnown("Faction", CORE.PC))
                {
                    Frame.color = CurrentLocation.OwnerCharacter.CurrentFaction.FactionColor;
                }
                else
                {
                    Frame.color = CORE.Instance.Database.DefaultFaction.FactionColor;
                }
            }
            else
            {
                Frame.color = CORE.Instance.Database.DefaultFaction.FactionColor;
            }

            Icon.sprite = CurrentLocation.CurrentProperty.Icon;

            TooltipTarget.SetTooltip(CurrentLocation.Name + " - Which belongs to "
                + (CurrentLocation.OwnerCharacter == null ?
                " no one."
                :
                (CurrentLocation.OwnerCharacter.Known.IsKnown("Name",CORE.PC)? CurrentLocation.OwnerCharacter.name : "- ???")));
        }
    }

    public void ShowLocationInfo()
    {
        LocationInfoUI.Instance.Show(CurrentLocation);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (CurrentLocation == null)
        {
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            SelectedPanelUI.Instance.Select(this.CurrentLocation);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            CurrentLocation.ShowActionMenu(transform);
        }
    }
}
