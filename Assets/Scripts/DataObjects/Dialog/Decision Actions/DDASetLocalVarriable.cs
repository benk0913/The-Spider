using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDASetLocalVarriable", menuName = "DataObjects/Dialog/Actions/DDASetLocalVarriable", order = 2)]
public class DDASetLocalVarriable : DialogDecisionAction
{
    public string Varriable;
    public string State;

    public override void Activate()
    {
        DialogWindowUI.Instance.SetDialogParameter(Varriable, State);
    }
}
