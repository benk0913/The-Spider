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

            int randomRumor = Random.Range(0, 100);

            if (randomRumor < 100)
            {
                rumor.name = targetChar.name + " in " + targetChar.CurrentLocation.CurrentProperty.name;
                rumor.Title = rumor.name;
                rumor.Description = targetChar.name + " is rumored to have been seen nearby " + targetChar.CurrentLocation.CurrentProperty.name;
                rumor.RelevantLocationID = targetChar.CurrentLocation.ID;
            }
            else if (randomRumor < 50)
            {
                Trait trait = targetChar.Traits[Random.Range(0, targetChar.Traits.Count)];

                rumor.name = targetChar.name + " is " + trait.name;
                rumor.Title = rumor.name;
                rumor.Description = targetChar.name + " is rumored to have the trait: " + trait;
                rumor.RelevantLocationID = "";
            }

            gatheredRumor = rumor;
        }
        else // USELESS RUMORS ->
        {
            Rumor rumor = Instantiate(CORE.Instance.Database.CustomRumor);

            int randomRumor = Random.Range(0, 100);

            if(randomRumor < 100)
            {
                LocationEntity randomLocation = CORE.Instance.GetRandomLocation();

                rumor.name = targetChar.name + " in " + randomLocation.CurrentProperty.name;
                rumor.Title = rumor.name;
                rumor.Description = targetChar.name + " is rumored to have been seen nearby " + randomLocation.CurrentProperty.name;
                rumor.RelevantLocationID = randomLocation.ID;
            }
            else if(randomRumor < 50)
            {
                Trait randomTrait = CORE.Instance.Database.GetRandomTrait();

                rumor.name = targetChar.name + " is " + randomTrait.name;
                rumor.Title = rumor.name;
                rumor.Description = targetChar.name + " is rumored to have the trait: " + randomTrait;
                rumor.RelevantLocationID = "";
            }

            gatheredRumor = rumor;
        }

        gatheredRumor.Description += "\n <size=15>Chance for this rumor to be valid: <color=yellow>"
                + Mathf.RoundToInt(100f*(awareValue/(awareValue+targetDiscreetValue)))+" % </color></size>";
        gatheredRumor.RelevantCharacterID = targetChar.ID;
        gatheredRumor.isTemporary = true;

        if (gatheredRumor != null)
        {
            CORE.Instance.SplineAnimationObject("EarCollectedWorld",
                character.CurrentLocation.transform,
                RumorsPanelUI.Instance.Notification.transform,
                ()=> { StatsViewUI.Instance.RefreshRumors(); },
                false);

            RumorsPanelUI.Instance.GainCustomRumor(gatheredRumor);
        }


        if (targetChar.IsKnown("CurrentLocation"))
        {
            return;
        }

        CORE.Instance.GenerateLongTermTask(this.Task, requester, character, character.CurrentLocation, targetChar);
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

        if (targetChar.IsKnown("CurrentLocation"))
        {
            reason = new FailReason("You already know where this person is.");
            return false;
        }

        if (!targetChar.IsKnown("Appearance") && !targetChar.IsKnown("Name"))
        {
            reason = new FailReason("You don't know either the NAME nor the LOOKS of this perosn.");
            return false;
        }

        return true;
    }
}
