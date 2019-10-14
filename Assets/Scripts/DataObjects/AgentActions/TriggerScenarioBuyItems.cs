using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TriggerScenarioBuyItems", menuName = "DataObjects/AgentActions/TriggerScenarioBuyItems", order = 2)]
public class TriggerScenarioBuyItems : TriggerScenario 
{
    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out string reason)
    {
        LocationEntity location = (LocationEntity)target;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if(!location.CurrentProperty.IsVendor)
        {
            return false;
        }

        return true;
    }
}
