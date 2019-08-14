﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SelectAgentPlayerAction", menuName = "DataObjects/PlayerActions/SelectAgentPlayerAction", order = 2)]
public class SelectAgentPlayerAction : PlayerAction
{
    public override void Execute(Character requester, AgentInteractable target)
    {
        if (!CanDoAction(requester, target))
        {
            GlobalMessagePrompterUI.Instance.Show("This agent is not yours to select.", 1f, Color.yellow);
            return;
        }

        PortraitUI agent = (PortraitUI)target;

        agent.SelectCharacter();
    }

    public override bool CanDoAction(Character requester, AgentInteractable target)
    {
        PortraitUI agent = (PortraitUI)target;

        if (agent.CurrentCharacter == requester)
        {
            return false;
        }

        if (agent.CurrentCharacter.TopEmployer != requester)
        {
            return false;
        }
        
        return true;
    }
}
