using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PurchasePlotAction", menuName = "DataObjects/AgentActions/PurchasePlotAction", order = 2)]
public class PurchasePlotAction : AgentAction
{

    public override void Execute(Character character, AgentInteractable target)
    {
        base.Execute(character, target);

        PurchasableEntity targetEntity = (PurchasableEntity)target;

        targetEntity.PurchasePlot(character);
    }
}
