using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ShowCharacterInfoPlayerAction", menuName = "DataObjects/PlayerActions/ShowCharacterInfoPlayerAction", order = 2)]
public class ShowCharacterInfoPlayerAction : PlayerAction
{
    public override void Execute(Character requester, AgentInteractable target)
    {
        FailReason reason;
        if (!CanDoAction(requester, target, out reason))
        {
            GlobalMessagePrompterUI.Instance.Show("This agent is not yours to select.", 1f, Color.yellow);
            return;
        }

        PortraitUI agent = (PortraitUI)target;

        CharacterInfoUI.Instance.ShowInfo(agent.CurrentCharacter);
    }

    public override bool CanDoAction(Character requester, AgentInteractable target, out FailReason reason)
    {
        reason = null;
        return true;
    }
}
