using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDCLocationHasEmployees", menuName = "DataObjects/Dialog/Conditions/DDCLocationHasEmployees", order = 2)]
public class DDCLocationHasEmployees : DialogDecisionCondition
{
    public int Amount = 1;
    public bool Guards = false;

    public override bool CheckCondition()
    {
        LocationEntity location = (LocationEntity)DialogWindowUI.Instance.GetDialogParameter("Location");

        int employeeInLocationCount = 0;

        foreach (Character character in location.CharactersInLocation)
        {
            if (Guards)
            {
                if (location.GuardsCharacters.Contains(character))
                {
                    employeeInLocationCount++;
                }
            }
            else
            {
                if (location.EmployeesCharacters.Contains(character))
                {
                    employeeInLocationCount++;
                }
            }
        }

        if (employeeInLocationCount < Amount)
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
