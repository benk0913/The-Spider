using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDCHasTrait", menuName = "DataObjects/Dialog/Conditions/DDCHasTrait", order = 2)]
public class DDCHasTrait : DialogDecisionCondition
{
    public string ParameterKey = "Actor";
    public Trait TheTrait;

    public override bool CheckCondition()
    {
        Character character = (Character)DialogWindowUI.Instance.GetDialogParameter(ParameterKey);

        if(!character.Traits.Contains(TheTrait))
        {
            if(Inverted)
            {
                return base.CheckCondition();
            }

            return false;
        }

        return base.CheckCondition();
    }
}
