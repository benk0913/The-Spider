using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ShowCharacterInfoPlayerAction", menuName = "DataObjects/PlayerActions/ShowCharacterInfoPlayerAction", order = 2)]
public class ShowCharacterInfoPlayerAction : PlayerAction
{
    public override void Execute(AgentInteractable target)
    {
        if (!CanDoAction(target))
        {
            GlobalMessagePrompterUI.Instance.Show("This agent is not yours to select.", 1f, Color.yellow);
            return;
        }

        PortraitUI agent = (PortraitUI)target;

        CharacterInfoUI.Instance.ShowInfo(agent.CurrentCharacter);
    }

    public override bool CanDoAction(AgentInteractable target)
    {
        return true;
    }
}
