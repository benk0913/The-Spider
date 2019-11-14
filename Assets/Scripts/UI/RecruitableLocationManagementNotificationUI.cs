using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecruitableLocationManagementNotificationUI : LocationManagementNotificationUI
{
    public override bool CommonLocationFilter(LocationEntity location)
    {
        return location.OwnerCharacter != null && location.OwnerCharacter.TopEmployer == CORE.PC
            && location.EmployeesCharacters.Count < location.CurrentProperty.PropertyLevels[location.Level - 1].MaxEmployees; ;
    }
}