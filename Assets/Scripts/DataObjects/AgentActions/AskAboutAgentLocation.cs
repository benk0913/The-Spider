using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AskAboutAgentLocation", menuName = "DataObjects/AgentActions/Spying/AskAboutAgentLocation", order = 2)]
public class AskAboutAgentLocation : AgentAction //DO NOT INHERIT FROM
{
    [SerializeField]
    LongTermTask Task;

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

        character.GoToRandomLocation();

        if (Random.Range(0, awareValue + targetDiscreetValue) < awareValue)
        {
            if (targetChar.CurrentLocation == character.CurrentLocation)
            {
                targetChar.Known.Know("CurrentLocation");
                targetChar.Known.Know("Appearance");
                TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance(character.name + " has found " + targetChar.name + "'s current location", ResourcesLoader.Instance.GetSprite("thumb-up"), character));

                return;
            }
            else if (targetChar.WorkLocation == character.CurrentLocation)
            {
                targetChar.Known.Know("CurrentLocation");
                targetChar.Known.Know("WorkLocation");
                targetChar.Known.Know("Appearance");
                TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance(character.name + " has found " + targetChar.name + "'s work location", ResourcesLoader.Instance.GetSprite("thumb-up"), character));

                return;
            }
            else if (targetChar.WorkLocation == character.HomeLocation)
            {
                if (Random.Range(0, awareValue + targetDiscreetValue) < awareValue) //EXTRA ROLL
                {
                    targetChar.Known.Know("CurrentLocation");
                    targetChar.Known.Know("HomeLocation");
                    targetChar.Known.Know("Appearance");
                    TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance(character.name + " has found " + targetChar.name + "'s home location", ResourcesLoader.Instance.GetSprite("thumb-up"), character));

                    return;
                }
            }
        }
    
        
        CORE.Instance.GenerateLongTermTask(this.Task, requester, character, character.CurrentLocation, targetChar);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out string reason)
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

        if (targetChar.TopEmployer == CORE.PC)
        {
            return false;
        }

        if (targetChar.IsKnown("CurrentLocation"))
        {
            reason = "You already know where this person is.";
            return false;
        }

        if (!targetChar.IsKnown("Appearance") && !targetChar.IsKnown("Name"))
        {
            reason = "You don't know either the NAME nor the LOOKS of this perosn.";
            return false;
        }

        return true;
    }
}
