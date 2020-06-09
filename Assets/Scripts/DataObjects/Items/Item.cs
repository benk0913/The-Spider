using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "DataObjects/Item", order = 2)]
public class Item : ScriptableObject
{
    [TextArea(3,6)]
    public string Description;
    public Sprite Icon;
    public int Price = 50;
    public bool Sellable = true;
    public bool Usable = true;
    public GameObject RealWorldPrefab;
    public List<AgentAction> ConsumeActions = new List<AgentAction>();

    public Item Clone()
    {
        Item newItem = Instantiate(this);
        newItem.name = this.name;

        return newItem;
    }
}
