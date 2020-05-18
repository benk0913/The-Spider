using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ForgeryCaseElement", menuName = "DataObjects/CaseElements/ForgeryCaseElement", order = 2)]
public class ForgeryCaseElement : ScriptableObject
{
    
    public Sprite Icon;

    [TextArea(3, 6)]
    public string Description;

    [TextArea(3, 6)]
    public string CostDescription;

    public int ConnectionsCost;
    public int RumorsCost;
    public int GoldCost;
    public int ProgressionCost;
    public int ReputationCost;
    public List<Item> RequiredItems = new List<Item>();
    public List<Trait> ReqTargetHasTraits = new List<Trait>();
    public int ReqTargetHasGender = -1;
    public List<string> ReqKnowItemsAboutTarget = new List<string>();
    public List<Trait> ReqAgentWithTraits = new List<Trait>();
    public List<BonusChallenge> ReqAgentWithBonus = new List<BonusChallenge>();
    public bool ReqTwoBookKeepers;
    public bool ReqRecentPlotSuccessful;
    public bool ReqPotentialAdulterAgent;

    public virtual void SpendCost()
    {
        CORE.PC.CGold -= GoldCost;
        CORE.PC.CConnections -= ConnectionsCost;
        CORE.PC.CRumors-= RumorsCost;
        CORE.PC.CProgress-= ProgressionCost;
        CORE.PC.Reputation-= ReputationCost;

        foreach(Item item in RequiredItems)
        {
            CORE.PC.Belogings.Remove(CORE.PC.Belogings.Find(x => x.name == item.name));
        }

        InventoryPanelUI.Instance.RefreshInventory();
    }

    public bool IsAvailable(AgentInteractable target, out FailReason reason) //LOBBY ELEMENT, DO NOT INHERIT FROM
    {
        reason = null;

        Character TargetCharacter = null;
        LocationEntity TargetLocation = null;

        if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
        {
            TargetCharacter = ((PortraitUI)target).CurrentCharacter;
            return IsAvailable(TargetCharacter,out reason);
        }
        else if (target.GetType() == typeof(LocationEntity) )
        {
            TargetLocation = ((LocationEntity)target);
            return IsAvailable(TargetLocation, out reason);
        }


        return true;
    }

    public virtual bool IsAvailable(Character target, out FailReason reason)
    {
        reason = null;

        if (!IsAvailableAgents(out reason))
        {
            return false;
        }

        if (!IsAvailableResources(out reason))
        {
            return false;
        }

        if (!IsAvailableTargetCharacter(target, out reason))
        {
            return false;
        }

        if (ReqPotentialAdulterAgent)
        {
            List<Character> agents = new List<Character>();
            agents.AddRange(CORE.PC.CharactersInCommand.FindAll(x => x.IsAgent));

            if (agents.Find(x => x.Gender != target.Gender && x.Traits.Find(y=>y.name == "Evil" || y.name == "Bad Moral Standards") != null) == null)
            {
                reason = new FailReason("You don't have an agent with the opposite sex to " + target.name+" which has the trait Bad Moral Standards / Evil");
                return false;
            }
        }


        return true;
    }

    public virtual bool IsAvailable(LocationEntity target, out FailReason reason)
    {
        reason = null;

        if (!IsAvailableAgents(out reason))
        {
            return false;
        }

        if (!IsAvailableResources(out reason))
        {
            return false;
        }

        if (!IsAvailableTargetLocation(target, out reason))
        {
            return false;
        }

        return true;
    }

    public bool IsAvailableResources(out FailReason reason)
    {
        reason = null;

        if(GoldCost > CORE.PC.CGold)
        {
            reason = new FailReason("Not Enough GOLD (" + CORE.PC.CGold + "/" + GoldCost + ")");
            return false;
        }

        if (ConnectionsCost > CORE.PC.CConnections)
        {
            reason = new FailReason("Not Enough CONNECTIONS (" + CORE.PC.CConnections + "/" + ConnectionsCost + ")");
            return false;
        }

        if (RumorsCost > CORE.PC.CRumors)
        {
            reason = new FailReason("Not Enough RUMORS (" + CORE.PC.CRumors + "/" + RumorsCost + ")");
            return false;
        }

        if (ProgressionCost > CORE.PC.CProgress)
        {
            reason = new FailReason("Not Enough PROGRESSION (" + CORE.PC.CProgress + "/" + ProgressionCost + ")");
            return false;
        }

        if (ReputationCost > CORE.PC.Reputation)
        {
            reason = new FailReason("Not Enough REPUTATION (" + CORE.PC.Reputation + "/" + ReputationCost + ")");
            return false;
        }

        foreach (Item item in RequiredItems)
        {
            if(CORE.PC.Belogings.Find(x=>x.name == item.name) == null)
            {
                reason = new FailReason("Does not have the item "+item.name);
                return false;
            }
        }

        return true;
    }

    public bool IsAvailableAgents(out FailReason reason)
    {
        reason = null;

        List<Character> agents = new List<Character>();
        agents.AddRange(CORE.PC.CharactersInCommand.FindAll(x => x.IsAgent));

        foreach(Trait trait in ReqAgentWithTraits)
        {
            if(agents.Find(x=>x.Traits.Contains(trait)) == null)
            {
                reason = new FailReason("You don't have an agent with the trait " + trait.name);
                return false;
            }
        }

        foreach (BonusChallenge bonus in ReqAgentWithBonus)
        {
            if (agents.Find(x => x.GetBonus(bonus.Type).Value >= bonus.ChallengeValue) == null)
            {
                reason = new FailReason("You don't have an agent with the skill " + bonus.Type.name +" above "+bonus.ChallengeValue);
                return false;
            }
        }

        List<LocationEntity> properties = new List<LocationEntity>();
        properties.AddRange(CORE.PC.PropertiesInCommand);

        if (ReqTwoBookKeepers)
        {
            List<LocationEntity> temp = new List<LocationEntity>();
            temp.AddRange(properties.FindAll(x => x.CurrentProperty.name == "Bookkeeper"));
            if(temp.Count < 2)
            {
                reason = new FailReason("You don't have two book keepers.");
                return false;
            }
        }

        if(ReqRecentPlotSuccessful)
        {

        }

        return true;
    }

    public bool IsAvailableTargetCharacter(Character targetCharacter , out FailReason reason)
    {
        reason = null;
        
        foreach (Trait trait in ReqTargetHasTraits)
        {
            if (targetCharacter.Traits.Find(x => x.name == trait.name) == null)
            {
                reason = new FailReason("Target does not have the trait " + trait.name);
                return false;
            }
        }

        if(ReqTargetHasGender != -1)
        {
            if ((int)targetCharacter.Gender != ReqTargetHasGender)
            {
                reason = new FailReason("Target is not " + ((GenderType)ReqTargetHasGender).ToString());
                return false;
            }
        }

        foreach(string knowledgeItem in ReqKnowItemsAboutTarget)
        {
            if(!targetCharacter.IsKnown(knowledgeItem, CORE.PC))
            {
                reason = new FailReason("You don't know the target's " + System.Text.RegularExpressions.Regex.Replace(knowledgeItem, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0"));
                return false;
            }
        }

        return true;
    }

    public bool IsAvailableTargetLocation(LocationEntity targetLocation, out FailReason reason)
    {
        reason = null;

        foreach (Trait trait in ReqTargetHasTraits)
        {
            if (targetLocation.Traits.Find(x => x.name == trait.name) == null)
            {
                reason = new FailReason("Target does not have the trait " + trait.name);
                return false;
            }
        }

        
        foreach (string knowledgeItem in ReqKnowItemsAboutTarget) //ABOUT LOCATION OWNER AND NOT LOCATION ITSELF!
        {
            if (!targetLocation.OwnerCharacter.Known.IsKnown(knowledgeItem, CORE.PC))
            {
                reason = new FailReason("You don't know the target owner's: " + System.Text.RegularExpressions.Regex.Replace(knowledgeItem, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0"));
                return false;
            }
        }

        return true;
    }
}
