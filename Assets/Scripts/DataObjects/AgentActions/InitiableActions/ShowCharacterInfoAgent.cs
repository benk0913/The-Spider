using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShowCharacterInfoAgent", menuName = "DataObjects/AgentActions/ShowCharacterInfoAgent", order = 2)]
public class ShowCharacterInfoAgent : AgentAction
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

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

        PortraitUI portrait = (PortraitUI)target;

        CharacterInfoUI.Instance.ShowInfo(portrait.CurrentCharacter);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out string reason)
    {
        if (!base.CanDoAction(requester, character, target, out reason))
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
