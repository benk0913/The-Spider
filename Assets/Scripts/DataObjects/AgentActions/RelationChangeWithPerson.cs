using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RelationChangeWithPerson", menuName = "DataObjects/AgentActions/RelationChangeWithPerson", order = 2)]
public class RelationChangeWithPerson : AgentAction //DO NOT INHERIT FROM
{
    public string RelationTitle;
    public int RelationAmount;
    public int RelationDuration;

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);


        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        targetChar.DynamicRelationsModifiers.Add(
             new DynamicRelationsModifier(
                 new RelationsModifier(RelationTitle, RelationAmount), RelationDuration, CORE.PC));

        CharacterInfoUI.Instance.RefreshUI();
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if(targetChar.TopEmployer != character.TopEmployer)
        {
            return false;
        }

        if (targetChar.Traits.Contains(CORE.Instance.Database.GetTrait("Good Moral Standards")) || targetChar.Traits.Contains(CORE.Instance.Database.GetTrait("Virtuous")))
        {
            reason = new FailReason(targetChar.name + " refuses to receive the gift. This act seems like a bribe. (Good Moral Standards)");
            return false;
        }


        return true;
    }
}
