using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentInteractable : MonoBehaviour
{

    public virtual List<AgentAction> GetPossibleAgentActions(Character forCharacter)
    {
        return null;
    }

    public virtual List<PlayerAction> GetPossiblePlayerActions()
    {
        return null;
    }

    public void ShowActionMenu(ActionCategory category = null)
    {
        if (ControlCharacterPanelUI.CurrentCharacter == null)
        {
            ShowPlayerActionMenu(null, category);
        }
        else
        {
            ShowAgentActionMenu(null, category);
        }
    }

    public void ShowActionMenu(Transform uniqueTransform, ActionCategory category = null)
    {
        if (ControlCharacterPanelUI.CurrentCharacter == null)
        {
            ShowPlayerActionMenu(uniqueTransform, category);
        }
        else
        {
            ShowAgentActionMenu(uniqueTransform, category);
        }
    }

    public void ShowAgentActionMenu(Transform uniqueTransform = null, ActionCategory category = null)
    {
        List<AgentAction> currentActions = new List<AgentAction>();
        currentActions.AddRange(GetPossibleAgentActions(ControlCharacterPanelUI.CurrentCharacter));
        List<DescribedAction> KeyActions = new List<DescribedAction>();

        if (category == null)
        { 
            List<ActionCategory> categories = new List<ActionCategory>();
            foreach (AgentAction action in currentActions)
            {
                if (action.Category == null)
                {
                    continue;
                }

                if (categories.Contains(action.Category))
                {
                    continue;
                }


                Character currentChar = ControlCharacterPanelUI.CurrentCharacter;
                FailReason reason = null;
                //TODO Replace requester
                if (!action.CanDoAction(CORE.PC, currentChar, this, out reason))
                {
                    if (reason == null)
                    {
                        continue;
                    }
                }

                categories.Add(action.Category);
            }

            foreach (ActionCategory relevantCategory in categories)
            {
                KeyActions.Add(
                    new DescribedAction(
                        relevantCategory.name,
                        () => ShowAgentActionMenu(uniqueTransform, relevantCategory)
                        , relevantCategory.Description
                        , relevantCategory.Icon
                        , true));
            }
        }

        currentActions.RemoveAll(x => x.Category != category);


        if (currentActions.Count == 0 && KeyActions.Count == 0)
        {
            return;
        }

        foreach (AgentAction action in currentActions)
        {
            FailReason reason = null;

            Character currentChar = ControlCharacterPanelUI.CurrentCharacter;

            //TODO REPLACE REQUESTER
            if (!action.CanDoAction(CORE.PC, currentChar, this, out reason) && reason == null)
            {
                continue;
            }

            KeyActions.Add(
                new DescribedAction(
                    action.name,
                    () => action.Execute(CORE.PC, currentChar, this)
                    , action.Description + (reason == null? "" : "\n <color=red>" + reason.Key.ToString() + "</color>")
                    , action.Icon
                    , action.CanDoAction(CORE.PC, currentChar, this, out reason)
                    , action.GetBonuses()));
        }

        if (KeyActions.Count == 0)
        {
            return;
        }

        RightClickDropDownPanelUI.Instance.Show(
            KeyActions,
            uniqueTransform == null? transform : uniqueTransform, 
            ControlCharacterPanelUI.CurrentCharacter, 
            this);
    }

    public void ShowPlayerActionMenu(Transform uniqueTransform = null, ActionCategory category = null)
    {

        List<PlayerAction> currentActions = new List<PlayerAction>();
        currentActions.AddRange(GetPossiblePlayerActions());

        List<DescribedAction> KeyActions = new List<DescribedAction>();

        if (category == null)
        {
            List<ActionCategory> categories = new List<ActionCategory>();
            foreach(PlayerAction action in currentActions)
            {
                if(action.Category == null)
                {
                    continue;
                }

                if(categories.Contains(action.Category))
                {
                    continue;
                }
                
                Character currentChar = ControlCharacterPanelUI.CurrentCharacter;
                FailReason reason = null;
                //TODO Replace requester
                if (!action.CanDoAction(CORE.PC, this, out reason))
                {
                    if (reason == null)
                    {
                        continue;
                    }
                }
                
                categories.Add(action.Category);
            }

            foreach(ActionCategory relevantCategory in categories)
            {
                KeyActions.Add(
                    new DescribedAction(
                        relevantCategory.name,
                        () => ShowPlayerActionMenu(uniqueTransform, relevantCategory)
                        , relevantCategory.Description
                        , relevantCategory.Icon
                        , true));
            }
        }

        currentActions.RemoveAll(x => x.Category != category);

        if (currentActions.Count == 0 && KeyActions.Count == 0)
        {
            return;
        }

        foreach (PlayerAction action in currentActions)
        {
            FailReason reason = null;

            //TODO Replace requester
            if (!action.CanDoAction(CORE.PC, this, out reason) && reason == null)
            {
                continue;
            }

            KeyActions.Add(
                new DescribedAction(
                    action.name,
                    () => action.Execute(CORE.PC, this)
                    , action.Description + (reason == null ? "" : "\n <color=red>" + reason.Key.ToString() + "</color>")
                    , action.Icon
                    , action.CanDoAction(CORE.PC, this, out reason)));
        }

        if (KeyActions.Count == 0)
        {
            return;
        }

        RightClickDropDownPanelUI.Instance.Show(
            KeyActions, 
            uniqueTransform==null? transform : uniqueTransform, 
            null, 
            this);
    }
}
