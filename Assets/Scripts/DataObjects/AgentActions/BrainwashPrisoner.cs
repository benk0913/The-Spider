using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BrainwashPrisoner", menuName = "DataObjects/AgentActions/Agression/Prisoners/BrainwashPrisoner", order = 2)]
public class BrainwashPrisoner : AgentAction //DO NOT INHERIT FROM
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        targetChar.AddTrait(CORE.Instance.Database.CultistTrait);
        targetChar.ExitPrison();

        TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance(targetChar.name + " was released from prison to join the cult.", ResourcesLoader.Instance.GetSprite("therapy"), targetChar));
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        reason = null;

        if (targetChar.PrisonLocation == null)
        {
            return false;
        }


        TechTreeItem tech = CORE.Instance.TechTree.Find(x => x.name == "Cult Practices");

        if (tech != null)
        {
            if (!tech.IsResearched)
            {
                return false;
            }
        }

        if (targetChar.PrisonLocation.OwnerCharacter == null)
        {
            Debug.LogError("No owner but still imprisoned?");
            return false;
        }

        if (targetChar.PrisonLocation.OwnerCharacter.TopEmployer != character.TopEmployer)
        {
            return false;
        }

        if (targetChar.PrisonLocation.OwnerCharacter.TopEmployer != CORE.PC)
        {
            return false;
        }

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }



        if(targetChar.Traits.Contains(CORE.Instance.Database.CultistTrait))
        {
            reason = new FailReason("This character is already a member of your cult.");
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
