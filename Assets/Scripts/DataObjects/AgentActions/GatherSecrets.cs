using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GatherSecrets", menuName = "DataObjects/AgentActions/Spying/GatherSecrets", order = 2)]
public class GatherSecrets : AgentAction //DO NOT INHERIT FROM
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        Character targetChar = ((PortraitUI)target).CurrentCharacter;
        targetChar.DynamicRelationsModifiers.Add(new DynamicRelationsModifier(
            new RelationsModifier("Shared Secrets!", 5)
            , 50
            , character
            ));
        character.DynamicRelationsModifiers.Add(new DynamicRelationsModifier(
            new RelationsModifier("Has told me secrets...", 5)
            , 50
            , targetChar
            ));

        targetChar.Known.Know("HomeLocation", character.TopEmployer);
        targetChar.Known.Know("WorkLocation", character.TopEmployer);
        targetChar.Known.Know("Personality", character.TopEmployer);
        targetChar.Known.Know("Name", character.TopEmployer);
        targetChar.Known.Know("Appearance", character.TopEmployer);
        targetChar.Known.Know("Faction", character.TopEmployer);

        if(targetChar.Employer != null)
        {
            targetChar.Employer.Known.Know("Name", character.TopEmployer);
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }


        int relations = targetChar.GetRelationsWith(character);
        float targetDiscreetValue = targetChar.GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value + 15;

        if (relations < targetDiscreetValue)
        {
            reason = new FailReason(
                targetChar.name + " doesn't like " + character.name + " enough... ("
                + relations + " \\ " + targetDiscreetValue + ")");
            return false;
        }

        return true;
    }
}
