using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDADecisionForTargetCharacter", menuName = "DataObjects/Dialog/Actions/DDADecisionForTargetCharacter", order = 2)]
public class DDADecisionForTargetCharacter : DialogDecisionAction
{

    public DialogPiece NextPiece;
    public DialogDecision DecisionPerCharacter;
    public DialogPiece PiecePerCharacter;

    public override void Activate()
    {
        LocationEntity location = (LocationEntity)DialogWindowUI.Instance.GetDialogParameter("Location");
        Character character = (Character)DialogWindowUI.Instance.GetDialogParameter("Target");

        DialogPiece piece = NextPiece.Clone();

        DialogDecision newDecision = DecisionPerCharacter.Clone();
        newDecision.name += " - " + character.name;

        DialogPiece newPiece = PiecePerCharacter.Clone();
        newPiece.TargetCharacters = new Character[] { character };

        newDecision.NextPiece = newPiece;

        piece.Decisions.Add(newDecision);

        DialogWindowUI.Instance.ShowDialogPiece(piece);
    }
}
