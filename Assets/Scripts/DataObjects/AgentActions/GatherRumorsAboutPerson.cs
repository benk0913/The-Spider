using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GatherRumorsAboutPerson", menuName = "DataObjects/AgentActions/Spying/GatherRumorsAboutPerson", order = 2)]
public class GatherRumorsAboutPerson : AgentAction //DO NOT INHERIT FROM
{
    [SerializeField]
    LongTermTask Task;

    [SerializeField]
    GameObject WinEffectPrefab;


    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        if (Task == null)
        {
            return;
        }

        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        float awareValue = character.GetBonus(CORE.Instance.Database.GetBonusType("Aware")).Value;
        float targetDiscreetValue = targetChar.GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value;

        
        if (Random.Range(0, awareValue + targetDiscreetValue) < awareValue)
        {
            targetChar.Known.Know("CurrentLocation", character.TopEmployer);
            character.GoToLocation(targetChar.CurrentLocation);
            CORE.Instance.ShowPortraitEffect(WinEffectPrefab, character, character.CurrentLocation);
        }
        else
        {
            CORE.Instance.ShowPortraitEffect(CORE.Instance.Database.FailWorldEffectPrefab, character, character.CurrentLocation);
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (requester != character && requester != CORE.Instance.Database.GOD && character.TopEmployer != requester)
        {
            return false;
        }

        if(targetChar == character)
        {
            return false;
        }

        if (targetChar.TopEmployer == character.TopEmployer)
        {
            return false;
        }

        if (targetChar.IsKnown("CurrentLocation", character.TopEmployer))
        {
            reason = new FailReason("You already know where this person is.");
            return false;
        }

        if (!targetChar.IsKnown("Appearance", character.TopEmployer) && !targetChar.IsKnown("Name", character.TopEmployer) && !targetChar.IsKnown("WorkLocation", character.TopEmployer))
        {
            reason = new FailReason("You don't know either the NAME, WORK LOCATION nor the LOOKS of this perosn.");
            return false;
        }

        return true;
    }
}
