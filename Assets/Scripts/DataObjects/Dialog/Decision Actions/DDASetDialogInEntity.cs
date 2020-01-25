using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDASetDialogInEntity", menuName = "DataObjects/Dialog/Actions/DDASetDialogInEntity", order = 2)]
public class DDASetDialogInEntity : DialogDecisionAction
{
    [SerializeField]
    DialogPiece DialogToSet;

    public override void Activate()
    {
        DialogEntity.Instance.SetDialog(DialogToSet);
    }
}
