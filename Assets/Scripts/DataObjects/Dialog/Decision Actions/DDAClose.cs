using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAClose", menuName = "DataObjects/Dialog/Actions/DDAClose", order = 2)]
public class DDAClose : DialogDecisionAction
{
    public override void Activate()
    {
        DialogWindowUI.Instance.HideCurrentDialog();
    }
}
