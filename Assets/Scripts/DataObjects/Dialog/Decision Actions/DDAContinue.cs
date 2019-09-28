using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAContinue", menuName = "DataObjects/Dialog/Actions/DDAContinue", order = 2)]
public class DDAContinue : DialogDecisionAction
{
    public override void Activate()
    {
        DialogWindowUI.Instance.ShowNextDialogPiece();
    }
}
