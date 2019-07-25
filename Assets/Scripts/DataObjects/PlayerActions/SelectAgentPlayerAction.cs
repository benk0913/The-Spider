using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SelectAgentPlayerAction", menuName = "DataObjects/PlayerActions/SelectAgentPlayerAction", order = 2)]
public class SelectAgentPlayerAction : PlayerAction
{
    public override void Execute(AgentInteractable target)
    {
        if (!CanDoAction(target))
        {
            GlobalMessagePrompterUI.Instance.Show("This agent is not yours to select.", 1f, Color.yellow);
            return;
        }

        PortraitUI agent = (PortraitUI)target;

        agent.SelectCharacter();
    }

    public override bool CanDoAction(AgentInteractable target)
    {
        PortraitUI agent = (PortraitUI)target;

        if(agent.CurrentCharacter.TopEmployer != CORE.PC)
        {
            return false;
        }
        
        return true;
    }
}
