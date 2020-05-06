using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AssaultPerson", menuName = "DataObjects/AgentActions/Agression/AssaultPerson", order = 2)]
public class AssaultPerson : AgentAction //DO NOT INHERIT FROM
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        targetChar.CurrentFaction.Relations.GetRelations(character.CurrentFaction).TotalValue -= 1;

        float charSTR = character.GetBonus(CORE.Instance.Database.GetBonusType("Strong")).Value;
        float targetSTR = targetChar.GetBonus(CORE.Instance.Database.GetBonusType("Strong")).Value;

        if(Random.Range(0f, (charSTR+targetSTR)) < charSTR)
        {
            character.GoToLocation(targetChar.CurrentLocation);
            targetChar.StopDoingCurrentTask(false);
            CORE.Instance.Database.GetAgentAction("Wounded").Execute(targetChar.TopEmployer, targetChar, targetChar.HomeLocation);

            if (character.TopEmployer == CORE.PC)
            {
                TurnReportUI.Instance.Log.Add(
                new TurnReportLogItemInstance(
                    character.name + " has assaulted " + targetChar.name,
                    ResourcesLoader.Instance.GetSprite("Satisfied"),
                    targetChar));
            }
        }
        else
        {
            character.GoToLocation(targetChar.CurrentLocation);
            CORE.Instance.ShowPortraitEffect(CORE.Instance.Database.FailWorldEffectPrefab, character, targetChar.CurrentLocation);
            CORE.Instance.Database.GetAgentAction("Wounded").Execute(character.TopEmployer, character, character.HomeLocation);
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if(targetChar.TopEmployer == character.TopEmployer)
        {
            return false;
        }

        if(!targetChar.IsKnown("CurrentLocation", character.TopEmployer))
        {
            reason = new FailReason("You don't know where this character is!");
        }

        if (character.Traits.Contains(CORE.Instance.Database.GetTrait("Good Moral Standards")) || character.Traits.Contains(CORE.Instance.Database.GetTrait("Virtuous")))
        {
            reason = new FailReason(character.name + " refuses. This act is too evil (Good Moral Standards)");
            return false;
        }


        return true;
    }
}
