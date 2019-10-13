using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SmugglingComplete", menuName = "DataObjects/AgentActions/Work/SmugglingComplete", order = 2)]
public class SmugglingComplete : WorkComplete
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        GenerateNewItemToProperty(character);
    }

    public void GenerateNewItemToProperty(Character character)
    {
        int inventoryCap = character.WorkLocation.CurrentProperty.PropertyLevels[character.WorkLocation.Level].InventoryCap;
        Item[] possibleItems = character.WorkLocation.CurrentProperty.PropertyLevels[character.WorkLocation.Level].PossibleMerchantise;

        Item newItem = Instantiate(possibleItems[Random.Range(0, possibleItems.Length)]);

        if (character.WorkLocation.Inventory.Count < inventoryCap)
        {
            character.WorkLocation.Inventory.Add(newItem);
        }
        else
        {
            character.WorkLocation.Inventory[Random.Range(0, character.WorkLocation.Inventory.Count)] = newItem;
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out string reason)
    {
        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (requester != CORE.Instance.Database.GOD && character.TopEmployer != requester && requester != character)
        {
            return false;
        }

        return true;
    }
}
