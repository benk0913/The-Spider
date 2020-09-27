using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CultistRevealLocationPlayerAction", menuName = "DataObjects/PlayerActions/CultistRevealLocationPlayerAction", order = 2)]
public class CultistRevealLocationPlayerAction : PlayerAction
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

        if (!targetChar.CurrentLocation.Known.IsKnown("Existance", requester))
        {
            targetChar.CurrentLocation.Known.Know("Existance", requester);
            CORE.Instance.Locations.FindAll(x => x.NearestDistrict == targetChar.CurrentLocation).ForEach(x => { x.RefreshState(); });
        }

        CORE.Instance.ShowHoverMessage("Cultist " + targetChar.name + " has revealed it's location!", ResourcesLoader.Instance.GetSprite("therapy"), targetChar.CurrentLocation.transform);

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
