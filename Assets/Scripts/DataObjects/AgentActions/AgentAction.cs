using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AgentAction", menuName = "DataObjects/AgentActions/AgentAction", order = 2)]
public class AgentAction : ScriptableObject
{
    public Sprite Icon;

    [TextArea(6, 10)]
    public string Description;

    public ActionCategory Category = null;

    public List<Trait> RequiredTraits = new List<Trait>();

    public int MinimumAge = 0;

    public int GoldCost;
    public int ConnectionsCost;
    public int RumorsCost;

    public TechTreeItem TechRequired;

    public Item ItemRequired;

    public BonusChallenge Challenge;

    public AgentAction FailureResult;

    public bool ShowHover = true;

    public LetterPreset employerLetterPreset;

    public GameObject WorldPortraitEffect;

    public List<TooltipBonus> TooltipBonuses = new List<TooltipBonus>();

    public AgentInteractable RecentTaret;

    public virtual void Execute(Character requester, Character character, AgentInteractable target)
    {
        RecentTaret = target;

        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            GlobalMessagePrompterUI.Instance.Show("This character can not do this action! ", 1f, Color.red);

            return;
        }

        if (!RollSucceed(character))
        {
            if (FailureResult != null)
            {
                FailureResult.Execute(requester, character, target);
            }
        }

        if (character.CurrentTaskEntity != null && character.CurrentTaskEntity.CurrentTask.Cancelable)
        {
            character.CurrentTaskEntity.Cancel();
        }

        //if (ShowHover && character.CurrentFaction == CORE.PC.CurrentFaction && target != null)
        //{
        //    CORE.Instance.ShowHoverMessage(this.name, null, target.transform);
        //}

        if (character.CurrentFaction == CORE.PC.CurrentFaction && target != null)
        {
            if (RumorsCost > 0)
            {
                CORE.Instance.ShowHoverMessage("<color=purple> Rumors -" + RumorsCost + "</color>", ResourcesLoader.Instance.GetSprite("earIcon"), target.transform);
            }

            if (GoldCost > 0)
            {
                CORE.Instance.ShowHoverMessage("<color=gold> Gold -" + GoldCost + "</color>", ResourcesLoader.Instance.GetSprite("icon_coins"), target.transform);
            }

            if (ConnectionsCost > 0)
            {
                CORE.Instance.ShowHoverMessage("<color=green> Connections -"+ConnectionsCost+"</color>",ResourcesLoader.Instance.GetSprite("connections"), target.transform);
            }
        }



        if (character.TopEmployer == CORE.PC)
        {
            if (employerLetterPreset != null)
            {
                if (character.Employer != null && character.TopEmployer != null && character.Employer != character.TopEmployer)
                {
                    Dictionary<string, object> letterParameters = new Dictionary<string, object>();

                    letterParameters.Add("Target_Name", character.name);
                    letterParameters.Add("Target_Role", character.CurrentRole);
                    letterParameters.Add("Letter_From", character.Employer);
                    letterParameters.Add("Letter_To", character.TopEmployer);
                    letterParameters.Add("Letter_SubjectCharacter", character);

                    LetterDispenserEntity.Instance.DispenseLetter(new Letter(employerLetterPreset, letterParameters));
                }
            }

            if (WorldPortraitEffect != null)
            {
                LocationEntity targetLocation;

                if (target.GetType() == typeof(LocationEntity))
                {
                    targetLocation = (LocationEntity)target;
                }
                else if (target.GetType() == typeof(PortraitUI))
                {
                    targetLocation = ((PortraitUI)target).CurrentCharacter.CurrentLocation;
                }
                else
                {
                    targetLocation = character.CurrentLocation;
                }

                CORE.Instance.ShowPortraitEffect(WorldPortraitEffect, character, targetLocation);


            }
        }

        CORE.Instance.DelayedInvokation(0.1f, () =>
        {
            requester.Gold -= GoldCost;
            requester.Connections -= ConnectionsCost;
            requester.Rumors -= RumorsCost;
        });
    }

    public virtual bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        reason = null;

        if (TechRequired != null)
        {
            TechTreeItem techInstance = CORE.Instance.TechTree.Find(x => x.name == TechRequired.name);

            if(techInstance != null && !techInstance.IsResearched)
            {
                return false;
            }
        }

        if(ItemRequired != null)
        {
            if(requester.Belogings.Find(x=>x.name == ItemRequired.name) == null)
            {
                reason = new FailReason("Requires The Item: "+ItemRequired.name);
                return false;
            }
        }

        if(target == null)
        {
            Debug.LogError("Target NULL?!!?!?!?");
            return false;
        }

        if(target.GetType() == typeof(PortraitUI))
        {
            Character targetCharacter = ((PortraitUI)target).CurrentCharacter;

            if (targetCharacter.IsDead)
            {
                return false;
            }
        }

        if(character.CurrentTaskEntity != null && !character.CurrentTaskEntity.CurrentTask.Cancelable)
        {
            reason = null;
            return false;
        }

        if (character.Age < MinimumAge)
        {
            reason = new FailReason("Too young to do so.");
            return false;
        }

        if (requester.Gold < GoldCost)
        {
            reason = new FailReason("Not Enough Gold");
            return false;
        }

        if (requester.Connections < ConnectionsCost)
        {
            reason = new FailReason("Not Enough Connections");
            return false;
        }

        if (requester.Rumors < RumorsCost)
        {
            reason = new FailReason("Not Enough Rumors");
            return false;
        }


        reason = null;
        return true;
    }

    public virtual bool RollSucceed(Character character)
    {
        if (this.Challenge == null || this.Challenge.Type == null)
        {
            return true;
        }

        float characterSkill = character.GetBonus(this.Challenge.Type).Value;
        float result = Random.Range(0f, characterSkill + Challenge.ChallengeValue + Challenge.RarityValue);


        bool finalResult = !Challenge.InvertedChance ? (characterSkill >= result) : (characterSkill < result); ;

        return finalResult;
    }

    public virtual List<TooltipBonus> GetBonuses()
    {
        List<TooltipBonus> bonuses = new List<TooltipBonus>();

        if(GoldCost > 0)
        {
            bonuses.Add(new TooltipBonus("Requires " + GoldCost + " Gold", ResourcesLoader.Instance.GetSprite("icon_coins")));
        }

        if (ConnectionsCost > 0)
        {
            bonuses.Add(new TooltipBonus("Requires " + ConnectionsCost + " Connections", ResourcesLoader.Instance.GetSprite("connections")));
        }

        if (RumorsCost > 0)
        {
            bonuses.Add(new TooltipBonus("Requires " + RumorsCost + " Rumors", ResourcesLoader.Instance.GetSprite("earIcon")));
        }

        return bonuses;
    }
}
