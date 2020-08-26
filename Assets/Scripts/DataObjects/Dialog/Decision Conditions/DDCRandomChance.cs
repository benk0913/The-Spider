using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDCRandomChance", menuName = "DataObjects/Dialog/Conditions/DDCRandomChance", order = 2)]
public class DDCRandomChance : DialogDecisionCondition
{
    public float Chance;

    public override bool CheckCondition()
    {
        if(Random.Range(0f,1f)<Chance)
        {
            if (Inverted)
            {
                return false;
            }

            return base.CheckCondition();
        }

        if (Inverted)
        {
            return base.CheckCondition(); ;
        }

        return false;
    }
}
