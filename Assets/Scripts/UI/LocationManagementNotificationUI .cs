using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationManagementNotificationUI : CharacterManagementNotificationUI
{
    protected List<LocationEntity> Locations = new List<LocationEntity>();

    protected override void RefreshElements()
    {
        this.Locations = CORE.Instance.Locations.FindAll(CommonLocationFilter);
    }

    public virtual bool CommonLocationFilter(LocationEntity location)
    {
        return true;
    }

    protected override void RefreshUI()
    {
        if (Locations.Count <= 0)
        {
            this.gameObject.SetActive(false);
            return;
        }

        this.gameObject.SetActive(true);

        RefreshTooltip();
    }

    protected override void RefreshTooltip()
    {
        List<TooltipBonus> Bonuses = new List<TooltipBonus>();

        for(int i=0;i< Locations.Count;i++)
        {
            if (CurrentIndex == i)
            {
                Bonuses.Add(new TooltipBonus("<color=green>"+Locations[i].CurrentProperty.name+"</color>", ResourcesLoader.Instance.GetSprite("pointing")));
            }
            else
            {
                Bonuses.Add(new TooltipBonus(Locations[i].CurrentProperty.name, ResourcesLoader.Instance.GetSprite("three-friends")));
            }
        }

        TooltipTarget.SetTooltip(ProblemText,Bonuses);
    }

    public override void ShowNextElement()
    {
        if(Locations.Count <= 0)
        {
            return;
        }

        CurrentIndex++;

        if(CurrentIndex >= Locations.Count)
        {
            CurrentIndex = 0;
        }

        RefreshUI();

        SelectedPanelUI.Instance.Select(Locations[CurrentIndex]);
    }
}
