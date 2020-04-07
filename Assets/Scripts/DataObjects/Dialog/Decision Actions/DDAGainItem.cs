using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAGainItem", menuName = "DataObjects/Dialog/Actions/DDAGainItem", order = 2)]
public class DDAGainItem : DialogDecisionAction
{
    [SerializeField]
    Item ItemToGain;

    public override void Activate()
    {
        CORE.PC.Belogings.Add(ItemToGain.Clone());
        InventoryPanelUI.Instance.ItemWasAdded(1);
    }
}
