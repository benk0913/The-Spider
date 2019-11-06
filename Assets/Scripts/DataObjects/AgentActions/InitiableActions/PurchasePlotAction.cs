using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PurchasePlotAction", menuName = "DataObjects/AgentActions/PurchasePlotAction", order = 2)]
public class PurchasePlotAction : AgentAction
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            return;
        }

        if(!RollSucceed(character))
        {
            if(FailureResult != null)
            {
                FailureResult.Execute(requester, character, target);
            }

            return;
        }

        PurchasableEntity targetEntity = (PurchasableEntity)target;

        targetEntity.PurchasePlot(requester, character);
    }
}
