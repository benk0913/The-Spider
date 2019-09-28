using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDCIsTimeOfDay", menuName = "DataObjects/Dialog/Conditions/DDCIsTimeOfDay", order = 2)]
public class DDCIsTimeOfDay : DialogDecisionCondition
{
    public GameClock.GameTime TimeOfDay;

    public override bool CheckCondition()
    {
        if(GameClock.Instance.CurrentTimeOfDay != TimeOfDay)
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
