using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDCLocalVarriable", menuName = "DataObjects/Dialog/Conditions/DDCLocalVarriable", order = 2)]
public class DDCLocalVarriable : DialogDecisionCondition
{
    public string Key;
    public string ValidValue;

    public override bool CheckCondition()
    {
        object parameter = DialogWindowUI.Instance.GetDialogParameter(Key);

        if(parameter == null)
        {
            if (Inverted)
            {
                return base.CheckCondition();
            }

            return false;
        }

        if (parameter.GetType() != typeof(string))
        {
            if(Inverted)
            {
                return base.CheckCondition();
            }

            return false;
        }

        if ((string)parameter != ValidValue)
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
