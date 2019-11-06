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

        targetChar.Known.Know("HomeLocation");
        targetChar.Known.Know("WorkLocation");
        targetChar.Known.Know("Personality");
        targetChar.Known.Know("Name");
        targetChar.Known.Know("Appearance");

        TurnReportUI.Instance.Log.Add(
            new TurnReportLogItemInstance(
                character.name + " has befriended " + targetChar.name, 
                ResourcesLoader.Instance.GetSprite("three-friends"), 
                targetChar));
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        return true;
    }
}
