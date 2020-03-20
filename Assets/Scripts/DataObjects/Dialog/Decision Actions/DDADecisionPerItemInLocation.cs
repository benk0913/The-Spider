using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDADecisionPerItemInLocation", menuName = "DataObjects/Dialog/Actions/DDADecisionPerItemInLocation", order = 2)]
public class DDADecisionPerItemInLocation : DialogDecisionAction
{
    public DialogPiece NextPiece;
    public DialogDecision DecisionsPerItem;
    public DialogPiece PiecePerItem;
    public DDABuyItem BuyAction;
    public DDCCanBuy BuyCondition;

    public override void Activate()
    {
        LocationEntity location = (LocationEntity)DialogWindowUI.Instance.GetDialogParameter("Location");

        DialogPiece piece = NextPiece.Clone();

        foreach (Item item in location.Inventory)
        {
            DialogDecision newDecision = DecisionsPerItem.Clone();
            newDecision.name += " - " + item.name + "("+item.Price+"/"+CORE.PC.CGold+")";
      
            piece.Decisions.Add(newDecision);
            newDecision.Icon = item.Icon;
            newDecision.NextPiece = PiecePerItem;

            DDABuyItem buyAction = Instantiate(BuyAction);
            buyAction.ItemToPurchase = item;
            newDecision.Actions.Insert(0, buyAction);

            DDCCanBuy buyCondition = Instantiate(BuyCondition);
            buyCondition.TheItem = item;
            buyCondition.name = BuyCondition.name;
            newDecision.ActiveConditions.Add(buyCondition);
        }

        DialogWindowUI.Instance.ShowDialogPiece(piece);
    }
}