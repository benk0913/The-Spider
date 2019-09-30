using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAReturnToLastLobbyPiece", menuName = "DataObjects/Dialog/Actions/DDAReturnToLastLobbyPiece", order = 2)]
public class DDAReturnToLastLobbyPiece : DialogDecisionAction
{
    public override void Activate()
    {
        DialogWindowUI.Instance.ShowDialogPiece(DialogWindowUI.Instance.LastLobbyPiece);
    }
}
