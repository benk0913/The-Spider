using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAUniqueActionForRelevantCharacter", menuName = "DataObjects/Dialog/Actions/DDAUniqueActionForRelevantCharacter", order = 2)]
public class DDAUniqueActionForRelevantCharacter : DialogDecisionAction
{
    [SerializeField]
    public AgentAction UniqueAction;

    public override void Activate()
    {
        if (DialogWindowUI.Instance.CurrentPiece.TargetCharacters == null || DialogWindowUI.Instance.CurrentPiece.TargetCharacters.Length == 0)
        {
            Debug.LogError("DDAUniqueActionForRelevantCharacter - CANT FIND CHARACTER FOR UNIQUE ACTION");
            return;
        }

        Character target = DialogWindowUI.Instance.CurrentPiece.TargetCharacters[0];

        Character actor = (Character)DialogWindowUI.Instance.GetDialogParameter("Actor");

        if (UniqueAction != null)
        {
            LocationEntity location = (LocationEntity)DialogWindowUI.Instance.GetDialogParameter("Location");

            UniqueAction.Execute(actor, target, location);
        }
    }
}
