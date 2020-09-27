using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerAction", menuName = "DataObjects/PlayerActions/PlayerAction", order = 2)]
public class PlayerAction : ScriptableObject
{
    [TextArea(2, 3)]
    public string Description;

    [SerializeField]
    public Sprite Icon;

    public ActionCategory Category = null;

    public TechTreeItem TechRequired;

    public int GoldCost = 0;
    public int RumorsCost = 0;
    public int ConnectionsCost= 0;
    public int ProgressionCost= 0;

    public virtual void Execute(Character requester, AgentInteractable target)
    {

        FailReason reason;
        if (!CanDoAction(requester, target, out reason))
        {
            GlobalMessagePrompterUI.Instance.Show("You cannot do this action!", 1f, Color.red);

            return;
        }

        requester.CGold -= GoldCost;
        requester.CRumors -= RumorsCost;
        requester.CConnections-= ConnectionsCost;
        requester.CProgress -= ProgressionCost;
    }

    public virtual bool CanDoAction(Character requester, AgentInteractable target, out FailReason reason)
    {
        reason = null;

        if (TechRequired != null)
        {
            TechTreeItem techInstance = CORE.Instance.TechTree.Find(x => x.name == TechRequired.name);

            if (techInstance != null && !techInstance.IsResearched)
            {
                return false;
            }
        }

        if(GoldCost > requester.CGold)
        {
            reason = new FailReason("Not Enough Gold");
            return false;
        }
        else if (RumorsCost > requester.CRumors)
        {
            reason = new FailReason("Not Enough Rumors");
            return false;
        }
        else if (ConnectionsCost > requester.CConnections)
        {
            reason = new FailReason("Not Enough Connections");
            return false;
        }
        else if (ProgressionCost > requester.CProgress)
        {
            reason = new FailReason("Not Enough Progression Points");
            return false;
        }


        return true;
    }

    public List<TooltipBonus> GetTooltipBonuses()
    {
        List<TooltipBonus> bonuses = new List<TooltipBonus>();

        if(GoldCost > 0)
        {
            bonuses.Add(new TooltipBonus("Gold Cost: " + GoldCost, ResourcesLoader.Instance.GetSprite("receive_money")));
        }

        if (RumorsCost > 0)
        {
            bonuses.Add(new TooltipBonus("Rumors Cost: " + RumorsCost, ResourcesLoader.Instance.GetSprite("earIcon")));
        }

        if (ConnectionsCost > 0)
        {
            bonuses.Add(new TooltipBonus("Connections Cost: " + ConnectionsCost, ResourcesLoader.Instance.GetSprite("connections")));
        }

        if (ProgressionCost > 0)
        {
            bonuses.Add(new TooltipBonus("Progression Cost: " + ProgressionCost, ResourcesLoader.Instance.GetSprite("scroll-unfurled")));
        }

        return bonuses;
    }
}

public class FailReason
{
    public string Key;
    public int Value;

    public FailReason(string key = "", int value = 0)
    {
        this.Key = key;
        this.Value = value;
    }
}
