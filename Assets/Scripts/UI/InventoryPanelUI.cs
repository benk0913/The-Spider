using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanelUI : MonoBehaviour, ISaveFileCompatible
{
    public static InventoryPanelUI Instance;

    public List<Item> Items = new List<Item>();

    [SerializeField]
    Transform itemsContainer;

    private void Awake()
    {
        Instance = this;
    }


    void OnEnable()
    {
        RefreshInventory();
    }

    public Item GetItem(string itemName)
    {
        foreach(Item item in Items)
        {
            if(item.name == itemName)
            {
                return item;
            }
        }

        return null;
    }

    public void AddNewItem(Item item)
    {
        Item newItem = Instantiate(item);
        newItem.name = item.name;
        Items.Add(newItem);
        AddItemToContainer(newItem);
    }

    public void Remove(Item item)
    {
        Items.Remove(item);
    }

    public void RefreshInventory()
    {
        ClearContainer();

        foreach (Item item in Items)
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

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        for (int i = 0; i < Items.Count; i++)
        {
            node["Items"][i] = Items[i].name;
        }

        return node;
    }

    public void FromJSON(JSONNode node)
    {
        Items.Clear();

        for (int i = 0; i < node["Items"].Count; i++)
        {
            Item item = Instantiate(CORE.Instance.Database.GetItem(node["Items"][i]));

            Items.Add(item);
        }

    }

    public void ImplementIDs()
    {
    }
}
