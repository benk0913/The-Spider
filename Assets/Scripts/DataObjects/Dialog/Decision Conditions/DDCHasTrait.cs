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
        object target = DialogWindowUI.Instance.GetDialogParameter(ParameterKey);

        if (target.GetType() == typeof(Character))
        {
            Character character = (Character)target;


            if (!character.Traits.Contains(TheTrait))
            {
                if (Inverted)
                {
                    return base.CheckCondition();
                }

                return false;
            }
        }
        else if (target.GetType() == typeof(LocationEntity))
        {
            LocationEntity location = (LocationEntity)target;

            if (!location.Traits.Contains(TheTrait))
            {
                if (Inverted)
                {
                    return base.CheckCondition();
                }

                return false;
            }
        }

        return base.CheckCondition();
    }
}
