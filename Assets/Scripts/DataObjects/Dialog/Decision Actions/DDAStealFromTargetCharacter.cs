using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAStealFromTargetCharacter", menuName = "DataObjects/Dialog/Actions/DDAStealFromTargetCharacter", order = 2)]
public class DDAStealFromTargetCharacter : DialogDecisionAction
{
    public string ThiefParameter = "Actor";

    public override void Activate()
    {
        if(DialogWindowUI.Instance.CurrentPiece.TargetCharacters == null || DialogWindowUI.Instance.CurrentPiece.TargetCharacters.Length == 0)
        {
            return;
        }

        Character actor = (Character)DialogWindowUI.Instance.GetDialogParameter(ThiefParameter);

        Item[] itemsStolen = DialogWindowUI.Instance.CurrentPiece.TargetCharacters[0].Belogings.ToArray();
        DialogWindowUI.Instance.CurrentPiece.TargetCharacters[0].Belogings.Clear();

        actor.Belogings.InsertRange(0, itemsStolen);
        CORE.PC.Belogings.InsertRange(0, itemsStolen);
        InventoryPanelUI.Instance.ItemWasAdded(itemsStolen.Length);

        DialogWindowUI.Instance.ShowItemsLooted(itemsStolen);
    }
}
