using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDCLocationHasEmployees", menuName = "DataObjects/Dialog/Conditions/DDCLocationHasEmployees", order = 2)]
public class DDCLocationHasEmployees : DialogDecisionCondition
{
    public override bool CheckCondition()
    {
        LocationEntity location = (LocationEntity)DialogWindowUI.Instance.GetDialogParameter("Location");

        int employeeInLocationCount = 0;

        foreach (Character character in location.CharactersInLocation)
        {
            if (location.EmployeesCharacters.Contains(character))
            {
                employeeInLocationCount++;
            }
        }

        if (employeeInLocationCount <= 1)
        {
            if (Inverted)
            {
                return base.CheckCondition();
            }

            return false;
        }

        if (Inverted)
        {
            return false;
        }

        return base.CheckCondition();
    }
}
