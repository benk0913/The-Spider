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

    public BonusType RequiredBonus = null;
    public int RequiredBonusValue = 0;

    public TechTreeItem TechRequired;

    public Item ItemRequired;
    public List<Item> MoreItemsRequired;

    public BonusChallenge Challenge;

    public AgentAction FailureResult;
    public AgentAction SuccessResult;

    public bool ShowHover = true;

    public LetterPreset employerLetterPreset;
    public LetterPreset characterLetterPreset;

    public GameObject WorldPortraitEffect;

    public GameObject WorldPortraitEffectOnTarget;

    public List<TooltipBonus> TooltipBonuses = new List<TooltipBonus>();

    public AgentInteractable RecentTaret;

    public PopupDataPreset OnExecutePopup;

    public bool ActionDoneByTarget = false;

    public string InvokeEventOnExecute = "";


    [SerializeField]
    bool CanDoInPrison = false;
    
    public bool CanTargetQuestionmarks = false;

    public virtual void Execute(Character requester, Character character, AgentInteractable target)
    {
        RecentTaret = target;

        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            if (character.TopEmployer == CORE.PC)
            {
                GlobalMessagePrompterUI.Instance.Show("This character can not do this action! " + reason?.Key, 2f, Color.red);

                if(CORE.Instance.DEBUG)
                {
                    Debug.Log("CAN NOT DO " + this.name);
                }
            }

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
                CORE.Instance.ShowHoverMessage("<color=purple> Rumors -" + RumorsCost + "</color>", ResourcesLoader.Instance.GetSprite("earIcon"), character.CurrentLocation.transform);
            }

            if (GoldCost > 0)
            {
                CORE.Instance.ShowHoverMessage("<color=yellow> Gold -" + GoldCost + "</color>", ResourcesLoader.Instance.GetSprite("icon_coins"), character.CurrentLocation.transform);
            }

            if (ConnectionsCost > 0)
            {
                CORE.Instance.ShowHoverMessage("<color=green> Connections -"+ConnectionsCost+"</color>",ResourcesLoader.Instance.GetSprite("connections"), character.CurrentLocation.transform);
            }
        }
       
        if (OnExecutePopup != null)
        {
            if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
            {
                Character targetCharacter = ((PortraitUI)target).CurrentCharacter;
                PopupWindowUI.Instance.AddPopup(new PopupData(OnExecutePopup,
                    new List<Character> { character },
                    new List<Character> { targetCharacter },
                    () =>
                    {
                        ExecuteResult(requester, character, target);
                    }));
            }
            else
            {
                PopupWindowUI.Instance.AddPopup(new PopupData(OnExecutePopup, new List<Character> { character },null, 
                    ()=> 
                    {
                        ExecuteResult(requester, character, target);
                    }));
            }
        }
        else
        {
            ExecuteResult(requester, character, target);
        }

        
    }

    public virtual void ExecuteResult(Character requester, Character character, AgentInteractable target)
    {
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

            if (characterLetterPreset != null)
            {
                if (character.Employer != null && character.TopEmployer != null && character != character.TopEmployer)
                {
                    Dictionary<string, object> letterParameters = new Dictionary<string, object>();

                    letterParameters.Add("Target_Name", character.name);
                    letterParameters.Add("Target_Role", character.CurrentRole);
                    letterParameters.Add("Letter_From", character);
                    letterParameters.Add("Letter_To", character.TopEmployer);
                    letterParameters.Add("Letter_SubjectCharacter", characterLetterPreset.PresetSubjectCharacter);

                    LetterDispenserEntity.Instance.DispenseLetter(new Letter(characterLetterPreset, letterParameters));
                }
            }

            if (WorldPortraitEffect != null)
            {
                LocationEntity targetLocation;

                if (target.GetType() == typeof(LocationEntity))
                {
                    targetLocation = (LocationEntity)target;
                }
                else if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
                {
                    targetLocation = ((PortraitUI)target).CurrentCharacter.CurrentLocation;
                }
                else
                {
                    targetLocation = character.CurrentLocation;
                }

                CORE.Instance.ShowPortraitEffect(WorldPortraitEffect, character, targetLocation);


            }

            if (WorldPortraitEffectOnTarget != null)
            {
                LocationEntity targetLocation;
                Character targetCharacter;

                targetCharacter = character;

                if (target.GetType() == typeof(LocationEntity))
                {
                    targetLocation = (LocationEntity)target;
                }
                else if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
                {
                    targetLocation = ((PortraitUI)target).CurrentCharacter.CurrentLocation;
                    targetCharacter = ((PortraitUI)target).CurrentCharacter;
                }
                else
                {
                    targetLocation = character.CurrentLocation;
                }

                CORE.Instance.ShowPortraitEffect(WorldPortraitEffectOnTarget, targetCharacter, targetLocation);


            }
        }

        CORE.Instance.DelayedInvokation(0.1f, () =>
        {
            requester.CGold -= GoldCost;
            requester.CConnections -= ConnectionsCost;
            requester.CRumors -= RumorsCost;

            if (ItemRequired != null)
            {
                requester.Belogings.Remove(requester.Belogings.Find(x => x.name == ItemRequired.name));
            }

            if(MoreItemsRequired.Count > 0)
            {
                MoreItemsRequired.ForEach((x) => { requester.Belogings.Remove(requester.Belogings.Find(y => y.name == x.name)); });
            }
        });

        if (SuccessResult != null)
        {
            SuccessResult.Execute(requester, character, target);
        }

        if (!string.IsNullOrEmpty(InvokeEventOnExecute))
        {
            CORE.Instance.InvokeEvent(InvokeEventOnExecute);
        }
    }

    public virtual bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        RecentTaret = target;
        Character targetCharacter = null;
        reason = null;

        if (TechRequired != null)
        {
            TechTreeItem techInstance = CORE.Instance.TechTree.Find(x => x.name == TechRequired.name);

            if(techInstance != null && !techInstance.IsResearched)
            {
                return false;
            }
        }

        if (ActionDoneByTarget)
        {
            if (target.GetType() == typeof(LocationEntity))
            {
                LocationEntity location = ((LocationEntity)target);
                if (location.OwnerCharacter != null && location.OwnerCharacter.TopEmployer != requester)
                {
                    return false;
                }
            }
            else if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
            {
                targetCharacter = ((PortraitUI)target).CurrentCharacter;

                if (targetCharacter.TopEmployer != requester)
                {
                    return false;
                }
            }
        }

        if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
        {
            targetCharacter = ((PortraitUI)target).CurrentCharacter;

            if (targetCharacter != null && targetCharacter.IsDead)
            {
                return false;
            }
        }
        else if (target.GetType() == typeof(LocationEntity))
        {
            if (((LocationEntity)target).VisibilityState == LocationEntity.VisibilityStateEnum.Hidden && !CanTargetQuestionmarks)
            {
                return false;
            }
            else if (((LocationEntity)target).VisibilityState == LocationEntity.VisibilityStateEnum.QuestionMark && !CanTargetQuestionmarks)
            {
                return false;
            }
        }

        if (ItemRequired != null)
        {
            if(requester.Belogings.Find(x=>x.name == ItemRequired.name) == null)
            {
                reason = new FailReason("Requires The Item: "+ItemRequired.name);
                return false;
            }
        }

        if (MoreItemsRequired.Count > 0)
        {
            foreach (Item requirement in MoreItemsRequired)
            {
                if (requester.Belogings.Find(x => x.name == requirement.name) == null)
                {
                    reason = new FailReason("Requires The Item: " + requirement.name);
                    return false;
                }
            }
        }

        if(target == null)
        {
            Debug.LogError("Target NULL?!!?!?!?");
            return false;
        }

        if (character.PrisonLocation != null && !CanDoInPrison)
        {
            reason = new FailReason("Character is in prison.");
            return false;
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

        if (requester != null)
        {
            if (requester.CGold < GoldCost)
            {
                if (character.TopEmployer == CORE.PC)
                {
                    CORE.Instance.ShowHoverMessage("Not enough Gold! "+"("+requester.CGold+"/"+GoldCost+")", ResourcesLoader.Instance.GetSprite("Unsatisfied"), character.CurrentLocation.transform);
                }

                reason = new FailReason("Not Enough Gold");
                return false;
            }

            if (requester.CConnections < ConnectionsCost)
            {
                reason = new FailReason("Not Enough Connections");

                if (character.TopEmployer == CORE.PC)
                {
                    CORE.Instance.ShowHoverMessage("Not enough Connections!" + "(" + requester.CConnections+ "/" + ConnectionsCost+ ")", ResourcesLoader.Instance.GetSprite("Unsatisfied"), character.CurrentLocation.transform);
                }

                return false;
            }

            if (requester.CRumors < RumorsCost)
            {
                reason = new FailReason("Not Enough Rumors");

                if (character.TopEmployer == CORE.PC)
                {
                    CORE.Instance.ShowHoverMessage("Not enough Rumors!" + "(" + requester.CRumors+ "/" + RumorsCost+ ")", ResourcesLoader.Instance.GetSprite("Unsatisfied"), character.CurrentLocation.transform);
                }

                return false;
            }
        }
        else
        {
            reason = new FailReason("No Requester (Error) [ sorry... ]");
            return false;
        }

        if(RequiredBonus != null)
        {
            float bonusValue = 0f;

            if (ActionDoneByTarget)
            {
                if (targetCharacter == null)
                {
                    Debug.LogError("NO CHARACTER " + this.name + " - " + character.name);
                }
                else
                {
                    bonusValue = targetCharacter.GetBonus(RequiredBonus).Value;
                }
            }
            else
            {
                bonusValue = character.GetBonus(RequiredBonus).Value;
            }

            if (bonusValue < RequiredBonusValue)
            {
                reason = new FailReason(RequiredBonus.name+" is too low! ("+bonusValue +"/"+RequiredBonusValue+")");
                return false;
            }
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
