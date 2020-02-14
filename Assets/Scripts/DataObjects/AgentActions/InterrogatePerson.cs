using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "InterrogatePerson", menuName = "DataObjects/AgentActions/Agression/Prisoners/InterrogatePerson", order = 2)]
public class InterrogatePerson : AgentAction //DO NOT INHERIT FROM
{
    [SerializeField]
    public InterrogationType Type;

    [SerializeField]
    public PopupDataPreset DeathPopup;

    [SerializeField]
    public PopupDataPreset WinPopup;

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        bool win = false;

        switch (Type)
        {
            case InterrogationType.Threaths://Your Reputation VS 10 + target discreet
                {
                    float targetValue = targetChar.GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value;

                    win = (Random.Range(2f, 10f + targetValue) < CORE.PC.Reputation);

                    break;
                }
            case InterrogationType.Pain://Your Intelligence VS Target's Strength - Can cause wound / death
                {
                    float offenseValue = character.GetBonus(CORE.Instance.Database.GetBonusType("Intelligent")).Value;
                    float targetValue  = targetChar.GetBonus(CORE.Instance.Database.GetBonusType("Strong")).Value;

                    win = (Random.Range(0f, (offenseValue + targetValue)) < offenseValue);

                    break;
                }
            case InterrogationType.Menace://Your menacing vs target menacing + discreet
                {
                    float offenseValue = character.GetBonus(CORE.Instance.Database.GetBonusType("Menacing")).Value;
                    float targetValue = targetChar.GetBonus(CORE.Instance.Database.GetBonusType("Menacing")).Value;
                    float targetBonus = targetChar.GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value;


                    win = (Random.Range(0f, (offenseValue + targetValue + targetBonus)) < offenseValue);

                    break;
                }
            case InterrogationType.Blackmail://Costs 40 rumors - 50% chance.
                {
                    win = Random.Range(0, 2) == 0;
                    break;
                }
        }

        character.GoToLocation(targetChar.CurrentLocation);

        if(win)
        {

            PopupWindowUI.Instance.AddPopup(new PopupData(DeathPopup, new List<Character>() { character }, new List<Character>() { targetChar },
                  () =>
                  {
                      for(int i=0;i<12;i++)
                      {
                          CORE.Instance.GainInformation(character.CurrentLocation.transform, targetChar);
                      }

                      CORE.Instance.ShowHoverMessage("Interrogation Succeed", ResourcesLoader.Instance.GetSprite("Satisfied"), character.CurrentLocation.transform);
                  }));
        }
        else
        {
            CORE.Instance.ShowHoverMessage("Interrogation Failed", ResourcesLoader.Instance.GetSprite("Unsatisfied"), character.CurrentLocation.transform);
            CORE.Instance.ShowPortraitEffect(CORE.Instance.Database.FailWorldEffectPrefab, character, targetChar.CurrentLocation);

            if (Type == InterrogationType.Pain)
            {
                if (Random.Range(0f, 1f) < 0.3f)
                {
                    if (DeathPopup != null)
                    {
                        PopupWindowUI.Instance.AddPopup(new PopupData(DeathPopup, new List<Character>() { character }, new List<Character>() { targetChar },
                            () =>
                            {
                                CORE.Instance.ShowHoverMessage(targetChar.name + " has died.", ResourcesLoader.Instance.GetSprite("Unsatisfied"), character.CurrentLocation.transform);
                                CORE.Instance.Database.GetEventAction("Death").Execute(CORE.Instance.Database.GOD, targetChar, targetChar.CurrentLocation);
                            }));
                    }
                    else
                    {
                        CORE.Instance.ShowHoverMessage(targetChar.name + " has died.", ResourcesLoader.Instance.GetSprite("Unsatisfied"), character.CurrentLocation.transform);
                        CORE.Instance.Database.GetEventAction("Death").Execute(CORE.Instance.Database.GOD, targetChar, targetChar.CurrentLocation);
                    }
                }
            }
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

                        chance = offenseValue / targetValue;
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
