using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShowCharacterInfoAgent", menuName = "DataObjects/AgentActions/ShowCharacterInfoAgent", order = 2)]
public class ShowCharacterInfoAgent : AgentAction
{

    public override void Execute(Character character, AgentInteractable target)
    {
        base.Execute(character, target);

        if (!CanDoAction(character, target))
        {
            return;
        }

        PortraitUI portrait = (PortraitUI)target;

        CharacterInfoUI.Instance.ShowInfo(portrait.CurrentCharacter);
    }

    public override bool CanDoAction(Character character, AgentInteractable target)
    {
        if(target == null)
        {
            return false;
        }

        return true;
    }
}
