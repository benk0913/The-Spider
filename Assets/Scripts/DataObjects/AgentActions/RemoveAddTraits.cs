using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RemoveAddTraits", menuName = "DataObjects/AgentActions/RemoveAddTraits", order = 2)]
public class RemoveAddTraits : AgentAction
{
    public List<Trait> TraitsToAdd;
    public List<Trait> TraitsToRemove;

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        foreach(Trait trait in TraitsToAdd)
        {
            character.AddTrait(trait);
        }

        foreach (Trait trait in TraitsToRemove)
        {
            character.RemoveTrait(trait);
        }
    }
}
