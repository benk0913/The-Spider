using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BefriendPerson", menuName = "DataObjects/AgentActions/Spying/BefriendPerson", order = 2)]
public class BefriendPerson : AgentAction //DO NOT INHERIT FROM
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        Character targetChar = ((PortraitUI)target).CurrentCharacter;
        targetChar.DynamicRelationsModifiers.Add(new DynamicRelationsModifier(
            new RelationsModifier("My Friend!", 10)
            , 50
            , character
            ));
        character.DynamicRelationsModifiers.Add(new DynamicRelationsModifier(
            new RelationsModifier("My Friend! (Or is it?)", 5)
            , 50
            , targetChar
            ));

        TurnReportUI.Instance.Log.Add(
            new TurnReportLogItemInstance(
                character.name + " has befriended " + targetChar.name, 
                ResourcesLoader.Instance.GetSprite("three-friends"), 
                targetChar));

        CORE.Instance.GainInformation(character.CurrentLocation.transform, targetChar);
        CORE.Instance.GainInformation(character.CurrentLocation.transform, targetChar);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (targetChar.UnManipulable)
        {
            reason = new FailReason("You can not manipulate "+targetChar.name+"...");
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


        return true;
    }
}
