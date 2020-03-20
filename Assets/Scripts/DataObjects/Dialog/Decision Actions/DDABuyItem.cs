using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDABuyItem", menuName = "DataObjects/Dialog/Actions/DDABuyItem", order = 2)]
public class DDABuyItem : DialogDecisionAction
{
    public Item ItemToPurchase;

    public override void Activate()
    {
        if(CORE.PC.CGold < ItemToPurchase.Price)
        {
            GlobalMessagePrompterUI.Instance.Show("Not enough gold to buy " + ItemToPurchase.name + " (" + CORE.PC.CGold + "/" + ItemToPurchase.Price + ")", 1f , Color.red);
            return;
        }


        AudioControl.Instance.Play("purchase");

        CORE.PC.CGold -= ItemToPurchase.Price;
        Item purchasedItem = Instantiate(ItemToPurchase);
        purchasedItem.name = ItemToPurchase.name;
        CORE.PC.Belogings.Add(purchasedItem);
        LocationEntity location = (LocationEntity)DialogWindowUI.Instance.GetDialogParameter("Location");
        location.Inventory.Remove(ItemToPurchase);
        InventoryPanelUI.Instance.ItemWasAdded(1);
    }
}
