using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RebrandableLocationManagementNotificationUI : LocationManagementNotificationUI
{
    public override bool CommonLocationFilter(LocationEntity location)
    {
        return location.OwnerCharacter != null && location.OwnerCharacter.TopEmployer == CORE.PC
            && location.CurrentProperty == location.CurrentProperty.PlotType.BaseProperty;
    }
}
