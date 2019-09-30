using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "DataObjects/Item", order = 2)]
public class Item : ScriptableObject
{
    public string Description;
    public Sprite Icon;

    public Item Clone()
    {
        Item newItem = Instantiate(this);
        newItem.name = this.name;

        return newItem;
    }
}
