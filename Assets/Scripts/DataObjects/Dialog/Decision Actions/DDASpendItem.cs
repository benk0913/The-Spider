using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDASpendItem", menuName = "DataObjects/Dialog/Actions/DDASpendItem", order = 2)]
public class DDASpendItem : DialogDecisionAction
{
    [SerializeField]
    Item ItemToSpend;

    public override void Activate()
    {
        CORE.PC.Belogings.Remove(CORE.PC.GetItem(ItemToSpend.name));
    }
}
