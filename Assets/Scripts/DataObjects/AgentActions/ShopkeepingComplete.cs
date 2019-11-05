using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopkeepingComplete", menuName = "DataObjects/AgentActions/Work/ShopkeepingComplete", order = 2)]
public class ShopkeepingComplete : WorkComplete
{
    public override void EarnGold(Character requester, Character character, AgentInteractable target, int addedGold = 0)
    {
        int inventoryCap = character.WorkLocation.CurrentProperty.PropertyLevels[character.WorkLocation.Level-1].InventoryCap;

        if (character.WorkLocation.Inventory.Count > 0 && Random.Range(0,2) == 0)
        {
            Item itemSold = character.WorkLocation.Inventory[Random.Range(0, character.WorkLocation.Inventory.Count)];

            character.WorkLocation.Inventory.Remove(itemSold);

            base.EarnGold(requester, character, target, itemSold.Price);

            Restock(character);
            return;
        }

        base.EarnGold(requester, character, target);
        Restock(character);
    }

    public void Restock(Character character)
    {
        int inventoryCap = character.WorkLocation.CurrentProperty.PropertyLevels[character.WorkLocation.Level-1].InventoryCap;

        if (character.WorkLocation.Inventory.Count >= inventoryCap)
        {
            return;
        }

        List<LocationEntity> SmuggleSources = CORE.Instance.Locations.FindAll((LocationEntity locInQuestion) =>
        {
            return 
            locInQuestion.CurrentProperty == CORE.Instance.Database.GetPropertyByName("Smuggler's Warehouse") 
            && locInQuestion.Inventory.Count > 0;
        });

        if(SmuggleSources.Count <= 0)
        {
            return;
        }

        LocationEntity randomLocation = SmuggleSources[Random.Range(0, SmuggleSources.Count)];
        Item randomItem = randomLocation.Inventory[Random.Range(0, randomLocation.Inventory.Count)];
        Item newItem = Instantiate(randomItem);
        newItem.name = randomItem.name;

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
