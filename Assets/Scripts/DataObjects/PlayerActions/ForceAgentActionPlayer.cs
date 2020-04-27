using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ForceAgentActionPlayer", menuName = "DataObjects/PlayerActions/ForceAgentActionPlayer", order = 2)]
public class ForceAgentActionPlayer : PlayerAction
{
    [SerializeField]
    public AgentAction ActionToForce;

    [SerializeField]
    public bool SelectAgent = true;

    public override void Execute(Character requester, AgentInteractable target)
    {
        FailReason reason;
        if (!CanDoAction(requester, target, out reason))
        {
            GlobalMessagePrompterUI.Instance.Show("You cannot do that."+reason?.Key, 2f, Color.yellow);
            return;
        }

        if (SelectAgent)
        {
            SelectAgentWindowUI.Instance.Show(
                (Character character) => { ActionToForce.Execute(requester, character, target); SelectedPanelUI.Instance.Deselect(); }
                , x => { return x.TopEmployer == CORE.PC && x.TopEmployer != x && x.IsAgent; },"Select Agent:",null,ActionToForce, target);
        }
        else
        {
            ActionToForce.Execute(requester, requester, target);
        }
    }

    public override bool CanDoAction(Character requester, AgentInteractable target, out FailReason reason)
    {

        reason = null;

        //if (TechRequired != null)
        //{
        //    TechTreeItem techInstance = CORE.Instance.TechTree.Find(x => x.name == TechRequired.name);

        //    if (techInstance != null && !techInstance.IsResearched)
        //    {
        //        return false;
        //    }
        //}

        //if (ActionToForce.ItemRequired != null)
        //{
        //    if (requester.Belogings.Find(x => x.name == ActionToForce.ItemRequired.name) == null)
        //    {
        //        reason = new FailReason("Requires The Item: " + ActionToForce.ItemRequired.name);
        //        return false;
        //    }
        //}

        //if (ActionToForce.MoreItemsRequired.Count > 0)
        //{
        //    foreach (Item requirement in ActionToForce.MoreItemsRequired)
        //    {
        //        if (requester.Belogings.Find(x => x.name == requirement.name) == null)
        //        {
        //            reason = new FailReason("Requires The Item: " + requirement.name);
        //            return false;
        //        }
        //    }
        //}

        //if (target == null)
        //{
        //    Debug.LogError("Target NULL?!!?!?!?");
        //    return false;
        //}

        //if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
        //{
        //    Character targetCharacter = ((PortraitUI)target).CurrentCharacter;

        //    if (targetCharacter.IsDead)
        //    {
        //        return false;
        //    }
        //}

        //if (requester.CGold < ActionToForce.GoldCost)
        //{
        //    if (requester.TopEmployer == CORE.PC)
        //    {
        //        CORE.Instance.ShowHoverMessage("Not enough Gold!", ResourcesLoader.Instance.GetSprite("Unsatisfied"), requester.CurrentLocation.transform);
        //    }

        //    reason = new FailReason("Not Enough Gold");
        //    return false;
        //}

        //if (requester.CConnections < ActionToForce.ConnectionsCost)
        //{
        //    reason = new FailReason("Not Enough Connections");

        //    if (requester.TopEmployer == CORE.PC)
        //    {
        //        CORE.Instance.ShowHoverMessage("Not enough Connections!", ResourcesLoader.Instance.GetSprite("Unsatisfied"), requester.CurrentLocation.transform);
        //    }

        //    return false;
        //}

        //if (requester.CRumors < ActionToForce.RumorsCost)
        //{
        //    reason = new FailReason("Not Enough Rumors");

        //    if (requester.TopEmployer == CORE.PC)
        //    {
        //        CORE.Instance.ShowHoverMessage("Not enough Rumors!", ResourcesLoader.Instance.GetSprite("Unsatisfied"), requester.CurrentLocation.transform);
        //    }

        //    return false;
        //}

        List<Character> agents = new List<Character>();
        agents.AddRange(CORE.PC.CharactersInCommand.FindAll(x => x.IsAgent));

        bool canDo = false;
        foreach(Character agent in agents)
        {
            if (ActionToForce.CanDoAction(requester, agent, target, out reason))
            {
                canDo = true;
                break;
            }
        }

        if(!canDo)
        {
            return false;
        }

        reason = null;
        return true;
    }
}
