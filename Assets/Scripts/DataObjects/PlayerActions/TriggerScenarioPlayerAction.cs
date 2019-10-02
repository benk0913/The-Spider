using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TriggerScenarioPlayerAction", menuName = "DataObjects/PlayerActions/TriggerScenarioPlayerAction", order = 2)]
public class TriggerScenarioPlayerAction : PlayerAction
{
    [SerializeField]
    SimpleScenario Scenario;

    public override void Execute(Character requester, AgentInteractable target)
    {
        string reason;
        if (!CanDoAction(requester, target, out reason))
        {
            GlobalMessagePrompterUI.Instance.Show("You cannot do that.", 1f, Color.yellow);
            return;
        }

        Character targetCharacter = ((PortraitUI)target).CurrentCharacter;


        SelectAgentWindowUI.Instance.Show(
          (Character character) =>      { Scenario.TriggerScenario(character, targetCharacter.CurrentLocation, targetCharacter); }
        , (Character charInQuestion) => { return charInQuestion.TopEmployer == CORE.PC && charInQuestion != CORE.PC; });
    }

    public override bool CanDoAction(Character requester, AgentInteractable target, out string reason)
    {
        reason = "";
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
        

        return true;
    }
}
