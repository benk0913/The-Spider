using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShowCharacterInfoAgent", menuName = "DataObjects/AgentActions/ShowCharacterInfoAgent", order = 2)]
public class ShowCharacterInfoAgent : AgentAction
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        if (!CanDoAction(requester, character, target))
        {
            return;
        }

        PortraitUI portrait = (PortraitUI)target;

        CharacterInfoUI.Instance.ShowInfo(portrait.CurrentCharacter);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target)
    {
        if (!base.CanDoAction(requester, character, target))
        {
            return false;
        }

        if (target == null)
        {
            return false;
        }

        return true;
    }
}
