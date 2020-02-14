using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AttemptInterrogatePerson ", menuName = "DataObjects/AgentActions/Agression/Prisoners/AttemptInterrogatePerson ", order = 2)]
public class AttemptInterrogatePerson : LongTermTaskExecuter //DO NOT INHERIT FROM
{
    [SerializeField]
    public InterrogationType Type;


    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        reason = null;

        if (targetChar.PrisonLocation == null)
        {
            return false;
        }

        if (!base.CanDoAction(requester, character, target, out reason))
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
        
        return base.CanDoAction(requester,character,target, out reason);
    }

    public override List<TooltipBonus> GetBonuses()
    {
        List<TooltipBonus> Bonuses = base.GetBonuses();

        float chance = 0f;
        switch (Type)
        {
            case InterrogationType.Threaths://Your Reputation VS 10 + target discreet
                {
                    if(RecentTaret != null && (RecentTaret.GetType() == typeof(PortraitUI) || RecentTaret.GetType() == typeof(PortraitUIEmployee)))
                    {
                        float targetValue = ((PortraitUI)RecentTaret).CurrentCharacter.GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value;

                        chance = (CORE.PC.Reputation - 2f) / (10f + targetValue);
                    }

                    break;
                }
            case InterrogationType.Pain://Your Intelligence VS Target's Strength - Can cause wound / death
                {
                    if (RecentTaret != null && (RecentTaret.GetType() == typeof(PortraitUI) || RecentTaret.GetType() == typeof(PortraitUIEmployee)))
                    {
                        List<Character> characters = CORE.PC.CharactersInCommand;

                        if(characters.Count == 0)
                        {
                            break;
                        }

                        float offenseValue = CORE.PC.CharactersInCommand[0].GetBonus(CORE.Instance.Database.GetBonusType("Intelligent")).Value;
                        float targetValue = ((PortraitUI)RecentTaret).CurrentCharacter.GetBonus(CORE.Instance.Database.GetBonusType("Strong")).Value;

                        chance = offenseValue / (offenseValue+targetValue);
                    }

                    break;
                }
            case InterrogationType.Menace://Your menacing vs target menacing + discreet
                {
                    if (RecentTaret != null && (RecentTaret.GetType() == typeof(PortraitUI) || RecentTaret.GetType() == typeof(PortraitUIEmployee)))
                    {
                        List<Character> characters = CORE.PC.CharactersInCommand;

                        if (characters.Count == 0)
                        {
                            break;
                        }

                        float offenseValue = CORE.PC.CharactersInCommand[0].GetBonus(CORE.Instance.Database.GetBonusType("Menacing")).Value;
                        float targetValue = ((PortraitUI)RecentTaret).CurrentCharacter.GetBonus(CORE.Instance.Database.GetBonusType("Menacing")).Value;
                        float targetBonus = ((PortraitUI)RecentTaret).CurrentCharacter.GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value;

                        chance = offenseValue / (targetValue+targetBonus);
                    }

                    break;
                }
            case InterrogationType.Blackmail://Costs 40 rumors - 50% chance.
                {
                    chance = 0.5f;
                    break;
                }
        }

        Bonuses.Add(new TooltipBonus(
            "Success: " + Mathf.RoundToInt(chance * 100f) + "%"
            , chance>0.5f? ResourcesLoader.Instance.GetSprite("thumb-up") : ResourcesLoader.Instance.GetSprite("thumb-down")));

        return Bonuses;
    }

    public enum InterrogationType
    {
        Threaths,
        Blackmail,
        Menace,
        Pain
    }
}
