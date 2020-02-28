using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAChanceBasedPiece", menuName = "DataObjects/Dialog/Actions/DDAChanceBasedPiece", order = 2)]
public class DDAChanceBasedPiece : DialogDecisionAction
{
    [Tooltip("These instances make a collective 1 (0.25,0.5 - means that theres 25% chance for the first and 25% for the second...)")]
    public List<DDAChanceBasedPieceInstance> Instances = new List<DDAChanceBasedPieceInstance>();
    public DialogPiece FallbackPiece;

    public override void Activate()
    {
        float random = Random.Range(0f, 1f);
        for(int i=0;i<Instances.Count;i++)
        {
            if(random < Instances[i].Chance)
            {
                DialogWindowUI.Instance.ShowDialogPiece(Instances[i].Piece);
                return;
            }
        }

        DialogWindowUI.Instance.ShowDialogPiece(FallbackPiece);
    }

    [System.Serializable]
    public class DDAChanceBasedPieceInstance
    {
        public DialogPiece Piece;
        public float Chance;
    }
}
