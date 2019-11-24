using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GenerateItemsAgentAction", menuName = "DataObjects/AgentActions/GenerateItemsAgentAction", order = 2)]
public class GenerateItemsAgentAction : AgentAction //DO NOT INHERIT FROM
{
    public List<Item> Items = new List<Item>();
    public bool random;

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        if(random)
        {
            targetChar.TopEmployer.Belogings.Add(Items[Random.Range(0, Items.Count)].Clone());
        }
        else
        {
            foreach(Item item in Items)
            {
                targetChar.TopEmployer.Belogings.Add(item.Clone());
            }
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        return true;
    }
}
