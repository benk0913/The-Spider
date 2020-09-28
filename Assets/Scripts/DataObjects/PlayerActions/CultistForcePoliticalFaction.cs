using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CultistForcePoliticalFaction", menuName = "DataObjects/PlayerActions/CultistForcePoliticalFaction", order = 2)]
public class CultistForcePoliticalFaction : PlayerAction
{

    public bool Loyalists = false;

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

        Trait oppositeTrait;
        if (Loyalists)
        {
            oppositeTrait = targetChar.Traits.Find(x => x.name == "Political Party - Rebels");
            if (oppositeTrait != null)
            {
                targetChar.DynamicRelationsModifiers.Add(new DynamicRelationsModifier(new RelationsModifier("Forced To Abandon Ideology.", -3), 10, CORE.PC));
            }

            targetChar.Traits.Remove(oppositeTrait);
            targetChar.AddTrait(CORE.Instance.Database.Traits.Find(X => X.name == "Political Party - Loyalists"));
        }
        else
        {
            oppositeTrait = targetChar.Traits.Find(x => x.name == "Political Party - Loyalists");
            if (oppositeTrait != null)
            {
                targetChar.DynamicRelationsModifiers.Add(new DynamicRelationsModifier(new RelationsModifier("Forced To Abandon Ideology.", -3), 10, CORE.PC));
            }

            targetChar.Traits.Remove(oppositeTrait);
            targetChar.AddTrait(CORE.Instance.Database.Traits.Find(X => X.name == "Political Party - Rebels"));
        }


        if (CharacterInfoUI.Instance.gameObject.activeInHierarchy)
        {
            CharacterInfoUI.Instance.ShowInfo(targetChar);
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

        if (Loyalists && character.Traits.Find(x => x.name == "Political Party - Loyalists") != null)
        {
            reason = new FailReason("This character is already a member of the loyalists faction!");
            return false;
        }
        else if (!Loyalists && character.Traits.Find(x => x.name == "Political Party - Rebels") != null)
        {
            reason = new FailReason("This character is already a member of the rebels faction!");
            return false;
        }

        if (!base.CanDoAction(requester,target,out reason))
        {
            return false;
        }

        return true;
    }
}
