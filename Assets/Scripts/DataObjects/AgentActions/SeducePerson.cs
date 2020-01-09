using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SeducePerson", menuName = "DataObjects/AgentActions/Spying/SeducePerson", order = 2)]
public class SeducePerson : AgentAction //DO NOT INHERIT FROM
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        Character targetChar = ((PortraitUI)target).CurrentCharacter;
        targetChar.DynamicRelationsModifiers.Add(new DynamicRelationsModifier(
            new RelationsModifier("My Lover!", 10)
            , 50
            , character
            ));
        character.DynamicRelationsModifiers.Add(new DynamicRelationsModifier(
            new RelationsModifier("My Lover! (Or is it?)", 5)
            , 50
            , targetChar
            ));

        targetChar.Known.Know("HomeLocation", character.TopEmployer);
        targetChar.Known.Know("WorkLocation", character.TopEmployer);
        targetChar.Known.Know("Personality", character.TopEmployer);
        targetChar.Known.Know("Name", character.TopEmployer);
        targetChar.Known.Know("Appearance", character.TopEmployer);

        if(targetChar.Traits.Contains(CORE.Instance.Database.GetTrait("Lustful")))
        {
            targetChar.Known.Know("Faction", character.TopEmployer);
        }

        TurnReportUI.Instance.Log.Add(
            new TurnReportLogItemInstance(
                character.name + " has seduced " + targetChar.name, 
                ResourcesLoader.Instance.GetSprite("Satisfied"), 
                targetChar));
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }


        int relations = targetChar.GetRelationsWith(character);
        float targetDiscreetValue = targetChar.GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value;

        if (relations < targetDiscreetValue)
        {

            reason = new FailReason(
                targetChar.name + " doesn't like " + character.name + " enough... ("
                + relations + " \\ " + targetDiscreetValue + ")");
            return false;
        }

        if (character.Gender == targetChar.Gender)
        {
            reason = new FailReason(targetChar.name + " is not attracted to " + character.name + "'s gender...");
            return false;
        }

        return true;
    }
}
