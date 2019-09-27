using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDCAontinue", menuName = "DataObjects/Dialog/Actions/DDCAontinue", order = 2)]
public class DDCAontinue : DialogDecisionAction
{
    public override void Activate()
    {
        DialogWindowUI.Instance.ShowNextDialogPiece();
    }
}
