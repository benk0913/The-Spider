using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemToInventryEntity : MonoBehaviour
{
    public List<Item> InventoryItem;
    public Item ItemToAdd;
    [SerializeField]
    GameObject TheObject;


    private void Start()
    {
        CORE.Instance.SubscribeToEvent("GameLoadComplete", Refresh);
    }

    void Refresh()
    {
        foreach (Item item in InventoryItem)
        {
            if (CORE.PC != null && CORE.PC.Belogings.Find(X => X.name == item.name) != null)
            {
                AlreadyHasItem();
                return;
            }
        }

        DoesntHaveItem();
    }

    public void Interact()
    {
        if(TheObject.activeInHierarchy)
        {
            CORE.PC.Belogings.Add(ItemToAdd.Clone());
            AlreadyHasItem();

            GlobalMessagePrompterUI.Instance.Show(ItemToAdd.name + " has been added to your inventory.", 2f, Color.green);
        }
    }

    void AlreadyHasItem()
    {
        TheObject.SetActive(false);
    }

    void DoesntHaveItem()
    {
        TheObject.SetActive(true);
    }
}
