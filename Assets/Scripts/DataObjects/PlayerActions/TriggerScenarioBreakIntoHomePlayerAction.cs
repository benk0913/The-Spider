using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TriggerScenarioBreakIntoHomePlayerAction", menuName = "DataObjects/PlayerActions/TriggerScenarioBreakIntoHomePlayerAction", order = 2)]
public class TriggerScenarioBreakIntoHomePlayerAction : PlayerAction
{
    [SerializeField]
    SimpleScenario Scenario;

    [SerializeField]
    LocationBasedScenario UniqueLocationScenarios;

    [SerializeField]
    LocationBasedScenario DistrictScenarios;

    public override void Execute(Character requester, AgentInteractable target)
    {
        FailReason reason;
        if (!CanDoAction(requester, target, out reason))
        {
            GlobalMessagePrompterUI.Instance.Show("You cannot do that.", 1f, Color.yellow);
            return;
        }

        Character targetCharacter = ((PortraitUI)target).CurrentCharacter;

        foreach (LocationScenarioInstance instance in UniqueLocationScenarios.LocationScenarios)
        {
            if (instance.PropertyType == targetCharacter.HomeLocation.CurrentProperty)
            {
                SelectAgentWindowUI.Instance.Show(
                      (Character character) => { UniqueLocationScenarios.TriggerScenario(character, targetCharacter.HomeLocation, targetCharacter); }
                    , (Character charInQuestion) => { return charInQuestion.TopEmployer == CORE.PC && charInQuestion != CORE.PC; });

                return;
            }
        }

        foreach (LocationScenarioInstance instance in DistrictScenarios.LocationScenarios)
        {
            if (instance.PropertyType == targetCharacter.HomeLocation.CurrentProperty)
            {
                SelectAgentWindowUI.Instance.Show(
                      (Character character) => { DistrictScenarios.TriggerScenario(character, targetCharacter.HomeLocation, targetCharacter); }
                    , (Character charInQuestion) => { return charInQuestion.TopEmployer == CORE.PC && charInQuestion != CORE.PC; });

                return;
            }
        }

        SelectAgentWindowUI.Instance.Show(
                     (Character character) => { Scenario.TriggerScenario(character, targetCharacter.HomeLocation, targetCharacter); }
                   , (Character charInQuestion) => { return charInQuestion.TopEmployer == CORE.PC && charInQuestion != CORE.PC; });
    }

    public override bool CanDoAction(Character requester, AgentInteractable target, out FailReason reason)
    {
        reason = null;
        PortraitUI portrait = ((PortraitUI)target);

        if (portrait.CurrentCharacter == null)
        {
            return false;
        }

        if (portrait.CurrentCharacter == requester)
        {
            return false;
        }

        if (portrait.CurrentCharacter.TopEmployer == requester)
        {
            return false;
        }
        
        if(!portrait.CurrentCharacter.IsKnown("HomeLocation", requester))
        {
            reason = new FailReason("You don't know where this person lives...");
            return false;
        }
        

        return true;
    }
}
