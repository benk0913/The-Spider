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

        targetChar.Known.Know("HomeLocation");
        targetChar.Known.Know("WorkLocation");
        targetChar.Known.Know("Personality");
        targetChar.Known.Know("Name");
        targetChar.Known.Know("Appearance");

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

        return true;
    }
}
