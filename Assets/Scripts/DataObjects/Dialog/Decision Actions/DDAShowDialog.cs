using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAShowDialog", menuName = "DataObjects/Dialog/Actions/DDAShowDialog", order = 2)]
public class DDAShowDialog : DialogDecisionAction
{
    [SerializeField]
    DialogPiece DialogToSet;

    public override void Activate()
    {
        DialogWindowUI.Instance.ShowDialogPiece(DialogToSet);
    }
}
