using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TriggerScenarioBreakIntoHome", menuName = "DataObjects/AgentActions/TriggerScenarioBreakIntoHome", order = 2)]
public class TriggerScenarioBreakIntoHome : TriggerScenario 
{
    public LocationBasedScenario UniqueLocationScenarios;
    public LocationBasedScenario DistrictScenarios;

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        string reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            return;
        }

        if (!RollSucceed(character))
        {
            if (FailureResult != null)
            {
                FailureResult.Execute(requester, character, target);
            }

            return;
        }

        PortraitUI charPortrait = (PortraitUI)target;

        foreach(LocationScenarioInstance instance in UniqueLocationScenarios.LocationScenarios)
        {
            if(instance.PropertyType == charPortrait.CurrentCharacter.HomeLocation.CurrentProperty)
            {
                UniqueLocationScenarios.TriggerScenario(character, character.HomeLocation, charPortrait.CurrentCharacter);
                return;
            }
        }

        foreach (LocationScenarioInstance instance in DistrictScenarios.LocationScenarios)
        {
            if (instance.PropertyType == charPortrait.CurrentCharacter.HomeLocation.CurrentProperty)
            {
                DistrictScenarios.TriggerScenario(character, character.HomeLocation, charPortrait.CurrentCharacter);
                return;
            }
        }
        
        Scenario.TriggerScenario(character, character.HomeLocation, charPortrait.CurrentCharacter);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out string reason)
    {
        reason = "";
        PortraitUI charPortrait = (PortraitUI)target;

        if (charPortrait.CurrentCharacter == character)
        {
            return false;
        }

        if (!charPortrait.CurrentCharacter.IsKnown("HomeLocation"))
        {
            reason = "You don't know where this person lives.";
            return false;
        }

        return true;
    }
}
