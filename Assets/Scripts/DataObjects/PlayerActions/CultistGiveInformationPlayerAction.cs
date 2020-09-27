using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CultistGiveInformationPlayerAction", menuName = "DataObjects/PlayerActions/CultistGiveInformationPlayerAction", order = 2)]
public class CultistGiveInformationPlayerAction : PlayerAction
{

    public override void Execute(Character requester, AgentInteractable target)
    {
        FailReason reason;
        if (!CanDoAction(requester, target, out reason))
        {
            GlobalMessagePrompterUI.Instance.Show("This is not a cultist.", 1f, Color.yellow);
            return;
        }

        base.Execute(requester, target);

        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        targetChar.Known.KnowEverything(requester);

        CORE.Instance.ShowHoverMessage("Cultist " + targetChar.name + " has told you everything about its self!", ResourcesLoader.Instance.GetSprite("therapy"), targetChar.CurrentLocation.transform);

        if (CharacterInfoUI.Instance.gameObject.activeInHierarchy)
        {
            CharacterInfoUI.Instance.ShowInfo(targetChar);
        }

        if (AllCharactersWindowUI.Instance.gameObject.activeInHierarchy)
        {
            AllCharactersWindowUI.Instance.Refresh();
        }
    }

    public override bool CanDoAction(Character requester, AgentInteractable target, out FailReason reason)
    {
        reason = null;

        Character character = ((PortraitUI)target).CurrentCharacter;

        if(character == null)
        {
            return false;
        }

        if(character.Traits.Find(x=>x==CORE.Instance.Database.CultistTrait) == null)
        {
            return false;
        }

        if(!base.CanDoAction(requester,target,out reason))
        {
            return false;
        }

        return true;
    }
}
