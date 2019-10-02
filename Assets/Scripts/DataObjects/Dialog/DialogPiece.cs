using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogPiece", menuName = "DataObjects/Dialog/DialogPiece", order = 2)]
public class DialogPiece : ScriptableObject
{
    [TextArea(6,12)]
    public string Description;

    public Sprite Image;

    public bool LobbyPiece = false;


    public List<DialogDecision> Decisions = new List<DialogDecision>();

    [TextArea(6, 12)]
    public string[] RandomDescriptions;

    public Character[] TargetCharacters;

    public DialogPiece Clone()
    {
        DialogPiece newPiece = Instantiate(this);
        newPiece.name = this.name;

        return newPiece;
    }

}
