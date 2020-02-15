using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAMultiAction", menuName = "DataObjects/Dialog/Actions/DDAMultiAction", order = 2)]
public class DDAMultiAction : DialogDecisionAction
{
    [SerializeField]
    public List<DialogDecisionAction> Actions = new List<DialogDecisionAction>();

    public override void Activate()
    {
        Actions.ForEach(x => x.Activate());
    }
}
