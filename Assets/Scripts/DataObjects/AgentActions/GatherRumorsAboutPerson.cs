using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GatherRumorsAboutPerson", menuName = "DataObjects/AgentActions/Spying/GatherRumorsAboutPerson", order = 2)]
public class GatherRumorsAboutPerson : AgentAction //DO NOT INHERIT FROM
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

        character.GoToLocation(character.WorkLocation);

        Rumor gatheredRumor = null;
        if (Random.Range(0, awareValue + targetDiscreetValue) < awareValue)
        {
            Rumor rumor = Instantiate(CORE.Instance.Database.CustomRumor);

            rumor.name = targetChar.name + " in " + targetChar.CurrentLocation.CurrentProperty.name;
            rumor.Title = rumor.name;
            rumor.Description = targetChar.name + " is rumored to have been seen nearby " + targetChar.CurrentLocation.CurrentProperty.name;
            rumor.RelevantLocationID = targetChar.CurrentLocation.ID;

            gatheredRumor = rumor;
        }
        else // USELESS RUMORS ->
        {
            Rumor rumor = Instantiate(CORE.Instance.Database.CustomRumor);

            int randomRumor = Random.Range(0, 100);

            if(randomRumor < 40)
            {
                LocationEntity randomLocation = CORE.Instance.GetRandomLocation();

                rumor.name = targetChar.name + " in " + randomLocation.CurrentProperty.name;
                rumor.Title = rumor.name;
                rumor.Description = targetChar.name + " is rumored to have been seen nearby " + randomLocation.CurrentProperty.name;
                rumor.RelevantLocationID = randomLocation.ID;
            }
            else if(randomRumor < 60)
            {
                Trait randomTrait = CORE.Instance.Database.GetRandomTrait();

                rumor.name = targetChar.name + " is " + randomTrait.name;
                rumor.Title = rumor.name;
                rumor.Description = targetChar.name + " is rumored to be " + randomTrait;
                rumor.RelevantLocationID = "";
            }
            else if(randomRumor < 70)
            {
                rumor.name = targetChar.name + " likes cats.";
                rumor.Title = rumor.name;
                rumor.Description = targetChar.name + "  is rumored to like cats.";
                rumor.RelevantLocationID = "";
            }
            else if(randomRumor < 80)
            {
                rumor.name = targetChar.name + " fancies elephants.";
                rumor.Title = rumor.name;
                rumor.Description = targetChar.name + "  is rumored to fancy... elephants!?";
                rumor.RelevantLocationID = "";
            }
            else if (randomRumor < 90)
            {
                rumor.name = targetChar.name + " is amazing.";
                rumor.Title = rumor.name;
                rumor.Description = targetChar.name + "  is rumored to have an amazing personality.";
                rumor.RelevantLocationID = "";
            }
            else if (randomRumor < 100)
            {
                rumor.name = targetChar.name + " is looking good.";
                rumor.Title = rumor.name;
                rumor.Description = targetChar.name + "  is rumored to be quite the good looking.";
                rumor.RelevantLocationID = "";
            }

            gatheredRumor = rumor;
        }

        gatheredRumor.Description += "\n <size=15>This rumor may be wrong or useless, it depends on how much "
                + character.name
                + " is 'Aware'! <color=yellow>(" + character.GetBonus(CORE.Instance.Database.GetBonusType("Aware")).Value + ")</color></size>";
        gatheredRumor.RelevantCharacterID = targetChar.ID;
        gatheredRumor.isTemporary = true;

        if (gatheredRumor != null)
        {
            CORE.Instance.SplineAnimationObject("EarCollectedWorld",
                character.CurrentLocation.transform,
                RumorsPanelUI.Instance.Notification.transform,
                null,
                false);

            RumorsPanelUI.Instance.GainCustomRumor(gatheredRumor);
        }


        if (targetChar.IsKnown("CurrentLocation"))
        {
            return;
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
