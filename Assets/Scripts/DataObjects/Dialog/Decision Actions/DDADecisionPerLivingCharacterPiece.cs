using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDADecisionPerLivingCharacterPiece", menuName = "DataObjects/Dialog/Actions/DDADecisionPerLivingCharacterPiece", order = 2)]
public class DDADecisionPerLivingCharacterPiece : DialogDecisionAction
{

    public DialogPiece NextPiece;
    public DialogDecision DecisionPerCharacter;
    public DialogPiece PiecePerCharacter;

    public override void Activate()
    {
        LocationEntity location = (LocationEntity)DialogWindowUI.Instance.GetDialogParameter("Location");

        DialogPiece piece = NextPiece.Clone();

        foreach (Character character in location.CharactersLivingInLocation)
        {
            DialogDecision newDecision = DecisionPerCharacter.Clone();
            newDecision.name += " - " + character.name;

            DialogPiece newPiece = PiecePerCharacter.Clone();
            newPiece.TargetCharacters = new Character[] { character };

            newDecision.NextPiece = newPiece;

            piece.Decisions.Add(newDecision);
        }

        DialogWindowUI.Instance.ShowDialogPiece(piece);
    }
}