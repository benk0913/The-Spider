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

    public void ShowActionMenu()
    {
        if (ControlCharacterPanelUI.CurrentCharacter == null)
        {
            ShowPlayerActionMenu();
        }
        else
        {
            ShowAgentActionMenu();
        }
    }

    public void ShowActionMenu(Transform uniqueTransform)
    {
        if (ControlCharacterPanelUI.CurrentCharacter == null)
        {
            ShowPlayerActionMenu(uniqueTransform);
        }
        else
        {
            ShowAgentActionMenu(uniqueTransform);
        }
    }

    public void ShowAgentActionMenu(Transform uniqueTransform = null)
    {

        List<AgentAction> currentActions = GetPossibleAgentActions(ControlCharacterPanelUI.CurrentCharacter);
        List<DescribedAction> KeyActions = new List<DescribedAction>();

        if(currentActions == null || currentActions.Count == 0)
        {
            return;
        }

        foreach (AgentAction action in currentActions)
        {
            FailReason reason = null;

            Character currentChar = ControlCharacterPanelUI.CurrentCharacter;

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

    public void ShowPlayerActionMenu(Transform uniqueTransform = null)
    {

        List<PlayerAction> currentActions = GetPossiblePlayerActions();
        List<DescribedAction> KeyActions = new List<DescribedAction>();

        if(currentActions == null || currentActions.Count == 0)
        {
            return;
        }

        foreach (PlayerAction action in currentActions)
        {
            FailReason reason = null;

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
