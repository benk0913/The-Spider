using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "FalseArrestPerson", menuName = "DataObjects/AgentActions/Constabulary/FalseArrestPerson", order = 2)]
public class FalseArrestPerson : AgentAction //DO NOT INHERIT FROM
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        float charSTR = character.GetBonus(CORE.Instance.Database.GetBonusType("Strong")).Value;
        float targetSTR = targetChar.GetBonus(CORE.Instance.Database.GetBonusType("Strong")).Value;

        character.TopEmployer.Reputation--;

        if (character.TopEmployer == CORE.PC)
        {
            TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance("Arresting people for no reason... Reputation -1",
            ResourcesLoader.Instance.GetSprite("pointing"),
            CORE.PC));
        }

        if (Random.Range(0f, (charSTR+targetSTR)) < charSTR)
        {
            character.GoToLocation(targetChar.CurrentLocation);
            targetChar.StopDoingCurrentTask(false);
            CORE.Instance.Database.GetAgentAction("Get Arrested").Execute(targetChar.TopEmployer, targetChar, target);

            if (character.TopEmployer == CORE.PC)
            {
                TurnReportUI.Instance.Log.Add(
                new TurnReportLogItemInstance(
                    character.name + " has arrested " + targetChar.name,
                    ResourcesLoader.Instance.GetSprite("Satisfied"),
                    targetChar));
            }
        }
        else
        {
            character.GoToLocation(targetChar.CurrentLocation);
            CORE.Instance.ShowPortraitEffect(CORE.Instance.Database.FailWorldEffectPrefab, character, targetChar.CurrentLocation);
            CORE.Instance.Database.GetAgentAction("Wounded").Execute(character.TopEmployer, character, character.HomeLocation);
            TurnReportUI.Instance.Log.Add(
                new TurnReportLogItemInstance(
                    character.name + " has failed arresting " + targetChar.name,
                    ResourcesLoader.Instance.GetSprite("Unsatisfied"),
                    targetChar));
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
            return false;
        }

        if (!targetChar.IsKnown("Name", character.TopEmployer))
        {
            reason = new FailReason("You don't know what this characters name is!");
            return false;
        }

        if (!targetChar.IsKnown("Appearance", character.TopEmployer))
        {
            reason = new FailReason("You don't know how this character looks like!");
            return false;
        }

        if (targetChar.TopEmployer == targetChar && targetChar.CurrentFaction.name != CORE.Instance.Database.DefaultFaction.name)
        {
            reason = new FailReason("You cannot trace this character... (Forget it.)");
            return false;
        }

        if (targetChar.CurrentFaction.name == "House Howund")
        {
            reason = new FailReason("You are not allowed to act against the lord.");
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
