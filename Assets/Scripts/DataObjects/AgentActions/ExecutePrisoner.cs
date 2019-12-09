using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ExecutePrisoner", menuName = "DataObjects/AgentActions/Agression/Prisoners/ExecutePrisoner", order = 2)]
public class ExecutePrisoner : AgentAction //DO NOT INHERIT FROM
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        CORE.Instance.ShowHoverMessage(targetChar.name+" has died.", ResourcesLoader.Instance.GetSprite("Unsatisfied"), character.CurrentLocation.transform);
        CORE.Instance.Database.GetEventAction("Death").Execute(CORE.Instance.Database.GOD, targetChar, targetChar.CurrentLocation);
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

        if(targetChar.PrisonLocation.OwnerCharacter.TopEmployer != character.TopEmployer)
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
