using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TriggerScenario", menuName = "DataObjects/AgentActions/TriggerScenario", order = 2)]
public class TriggerScenario : AgentAction 
{
    public SimpleScenario Scenario;

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        string reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            return;
        }

        if (!RollSucceed(character))
        {
            if (FailureResult != null)
            {
                FailureResult.Execute(requester, character, target);
            }

            return;
        }

        base.Execute(requester, character, target);

        LocationEntity location = (LocationEntity)target;

        Scenario.TriggerScenario(character, location, location.OwnerCharacter );
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out string reason)
    {
        LocationEntity location = (LocationEntity)target;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        return true;
    }
}
