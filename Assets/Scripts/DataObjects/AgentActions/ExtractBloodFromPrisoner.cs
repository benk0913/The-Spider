using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ExtractBloodFromPrisoner", menuName = "DataObjects/AgentActions/Agression/Prisoners/ExtractBloodFromPrisoner", order = 2)]
public class ExtractBloodFromPrisoner : AgentAction //DO NOT INHERIT FROM
{
    public Item ItemToAdd;
    public int Count = 1;

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        CORE.Instance.ShowHoverMessage(targetChar.name+" has died.", ResourcesLoader.Instance.GetSprite("Unsatisfied"), character.CurrentLocation.transform);
        CORE.Instance.Database.GetEventAction("Death").Execute(CORE.Instance.Database.GOD, targetChar, targetChar.CurrentLocation);

        for (int i = 0; i < Count; i++)
        {
            CORE.PC.Belogings.Add(ItemToAdd);
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (targetChar.PrisonLocation == null)
        {
            return false;
        }

        if (targetChar.PrisonLocation.OwnerCharacter == null)
        {
            Debug.LogError("No owner but still imprisoned?");
            return false;
        }

        if (targetChar.NeverDED)
        {
            reason = new FailReason("You are not allowed to execute this character.");
            return false;
        }

        if (targetChar.PrisonLocation.OwnerCharacter.TopEmployer != character.TopEmployer)
        {
            return false;
        }

        if (character.Traits.Contains(CORE.Instance.Database.GetTrait("Good Moral Standards")) || character.Traits.Contains(CORE.Instance.Database.GetTrait("Virtuous")))
        {
            reason = new FailReason(character.name + " refuses. This act is too evil (Good Moral Standards)");
            return false;
        }


        return true;
    }
}
