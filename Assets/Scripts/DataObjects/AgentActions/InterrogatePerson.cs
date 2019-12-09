using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "InterrogatePerson", menuName = "DataObjects/AgentActions/Agression/Prisoners/InterrogatePerson", order = 2)]
public class InterrogatePerson : AgentAction //DO NOT INHERIT FROM
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        float charMenacing = character.GetBonus(CORE.Instance.Database.GetBonusType("Menacing")).Value;
        float targetDiscreet = targetChar.GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value;

        character.GoToLocation(targetChar.CurrentLocation);

        if(Random.Range(0f, (charMenacing+targetDiscreet)) < charMenacing)
        {
            targetChar.Known.KnowEverything(character.TopEmployer);
            CORE.Instance.ShowHoverMessage("Interrogation Succeed", ResourcesLoader.Instance.GetSprite("Satisfied"), character.CurrentLocation.transform);
        }
        else
        {
            CORE.Instance.ShowHoverMessage("Interrogation Failed", ResourcesLoader.Instance.GetSprite("Unsatisfied"), character.CurrentLocation.transform);
            CORE.Instance.ShowPortraitEffect(CORE.Instance.Database.FailWorldEffectPrefab, character, targetChar.CurrentLocation);
        }
        
        if(Random.Range(0f,1f) < 0.3f)
        {
            CORE.Instance.ShowHoverMessage(targetChar.name+" has died.", ResourcesLoader.Instance.GetSprite("Unsatisfied"), character.CurrentLocation.transform);
            CORE.Instance.Database.GetEventAction("Death").Execute(CORE.Instance.Database.GOD, targetChar, targetChar.CurrentLocation);
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (targetChar.PrisonLocation == null)
        {
            return false;
        }

        if (targetChar.PrisonLocation.OwnerCharacter == null)
        {
            Debug.LogError("No owner but still imprisoned?");
            return false;
        }

        if(targetChar.PrisonLocation.OwnerCharacter.TopEmployer != character.TopEmployer)
        {
            return false;
        }

        if (character.Traits.Contains(CORE.Instance.Database.GetTrait("Good Moral Standards")) || character.Traits.Contains(CORE.Instance.Database.GetTrait("Virtuous")))
        {
            reason = new FailReason(character.name + " refuses. This act is too evil (Good Moral Standards)");
            return false;
        }


        return true;
    }

    public override List<TooltipBonus> GetBonuses()
    {
        List<TooltipBonus> Bonuses = base.GetBonuses();

        Bonuses.Add(new TooltipBonus("Success: <color=yellow>Menacing</color> VS <color=red>Discreet</color>", ResourcesLoader.Instance.GetSprite("thumb-up")));
        Bonuses.Add(new TooltipBonus("Prisoner Dies: <color=yellow>30%</color>", ResourcesLoader.Instance.GetSprite("thumb-down")));

        return Bonuses;
    }
}
