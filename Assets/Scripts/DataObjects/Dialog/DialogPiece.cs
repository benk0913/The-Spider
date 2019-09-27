using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogPiece", menuName = "DataObjects/Dialog/DialogPiece", order = 2)]
public class DialogPiece : ScriptableObject
{
    [TextArea(6,12)]
    public string Description;

    public Sprite Image;

    public List<DialogDecision> Decisions = new List<DialogDecision>();
}
