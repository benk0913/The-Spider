using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanelUI : MonoBehaviour
{
    public static InventoryPanelUI Instance;

    [SerializeField]
    Transform itemsContainer;

    [SerializeField]
    public NotificationUI Notification;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("InventoryChanged", RefreshInventory);
    }


    void OnEnable()
    {
        RefreshInventory();
        StartCoroutine(RefreshInterval());
    }

    IEnumerator RefreshInterval()
    {
        int previousCount = 0;
        while(true)
        {
            yield return 0;
            if (previousCount != CORE.PC.Belogings.Count)
            {
                RefreshInventory();
                previousCount = CORE.PC.Belogings.Count;
            }
        }
    }


    public Item GetItem(string itemName)
    {
        foreach(Item item in CORE.PC.Belogings)
        {
            if(item.name == itemName)
            {
                return item;
            }
        }

        return null;
    }

    public void ItemWasAdded(int amount = 1)
    {
        RefreshInventory();

        if(amount == 0)
        {
            return;
        }

        if (!this.gameObject.activeInHierarchy)
        {
            Notification.Add(amount);
        }
    }

    public void RefreshInventory()
    {
        ClearContainer();

        if(CORE.PC == null)
        {
            return;
        }

        Notification.Wipe();

        foreach (Item item in CORE.PC.Belogings)
        {
            AddItemToContainer(item);
        }
    }

    void AddItemToContainer(Item item)
    {
        GameObject rumorPanel = ResourcesLoader.Instance.GetRecycledObject("ItemUI");
        rumorPanel.transform.SetParent(itemsContainer, false);
        rumorPanel.GetComponent<ItemUI>().SetInfo(item);
    }

    void ClearContainer()
    {
        while (itemsContainer.childCount > 0)
        {
            itemsContainer.GetChild(0).gameObject.SetActive(false);
            itemsContainer.GetChild(0).SetParent(transform);
        }
    }
}
