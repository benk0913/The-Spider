using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDCLocationHasPeople", menuName = "DataObjects/Dialog/Conditions/DDCLocationHasPeople", order = 2)]
public class DDCLocationHasPeople : DialogDecisionCondition
{
    public override bool CheckCondition()
    {
        LocationEntity location = (LocationEntity)DialogWindowUI.Instance.GetDialogParameter("Location");

        if(location.CharactersInLocation.Count <= 1)
        {
            if(Inverted)
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
