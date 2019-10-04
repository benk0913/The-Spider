using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BefriendPersonFailed", menuName = "DataObjects/AgentActions/Spying/BefriendPersonFailed", order = 2)]
public class BefriendPersonFailed : AgentAction //DO NOT INHERIT FROM
{
    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        Character targetChar = ((PortraitUI)target).CurrentCharacter;
        targetChar.DynamicRelationsModifiers.Add(new DynamicRelationsModifier(
            new RelationsModifier("Why is this person bothering me?", -2)
            , 50
            , character
            ));
        character.DynamicRelationsModifiers.Add(new DynamicRelationsModifier(
            new RelationsModifier("This person doesn't want to be friend.", -2)
            , 50
            , targetChar
            ));

        targetChar.Known.Know("Personality");
        targetChar.Known.Know("Name");
        targetChar.Known.Know("Appearance");

        TurnReportUI.Instance.Log.Add(
            new TurnReportLogItemInstance(
                character.name + " has befriended " + targetChar.name, 
                ResourcesLoader.Instance.GetSprite("three-friends"), 
                targetChar));
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out string reason)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        return true;
    }
}
