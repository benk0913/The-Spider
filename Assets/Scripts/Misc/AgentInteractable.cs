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

    public void ShowAgentActionMenu()
    {

        List<AgentAction> currentActions = GetPossibleAgentActions(ControlCharacterPanelUI.CurrentCharacter);
        List<DescribedAction> KeyActions = new List<DescribedAction>();

        if(currentActions == null || currentActions.Count == 0)
        {
            return;
        }

        foreach (AgentAction action in currentActions)
        {
            string reason = "";
            
            if(!action.CanDoAction(CORE.PC, ControlCharacterPanelUI.CurrentCharacter, this, out reason) && string.IsNullOrEmpty(reason))
            {
                continue;
            }

            KeyActions.Add(
                new DescribedAction(
                    action.name,
                    () => action.Execute(CORE.PC, ControlCharacterPanelUI.CurrentCharacter, this)
                    , action.Description + "\n <color=red>" + reason + "</color>"
                    , action.Icon
                    , action.CanDoAction(CORE.PC, ControlCharacterPanelUI.CurrentCharacter, this, out reason)));
        }

        if (KeyActions.Count == 0)
        {
            return;
        }

        RightClickDropDownPanelUI.Instance.Show(KeyActions, transform, ControlCharacterPanelUI.CurrentCharacter, this);
    }

    public void ShowPlayerActionMenu()
    {

        List<PlayerAction> currentActions = GetPossiblePlayerActions();
        List<DescribedAction> KeyActions = new List<DescribedAction>();

        if(currentActions == null || currentActions.Count == 0)
        {
            return;
        }

        foreach (PlayerAction action in currentActions)
        {
            string reason = "";

            if (!action.CanDoAction(CORE.PC, this, out reason) && string.IsNullOrEmpty(reason))
            {
                continue;
            }

            KeyActions.Add(
                new DescribedAction(
                    action.name,
                    () => action.Execute(CORE.PC, this)
                    , action.Description + "\n <color=red>" + reason + "</color>"
                    , action.Icon
                    , action.CanDoAction(CORE.PC, this, out reason)));
        }

        if (KeyActions.Count == 0)
        {
            return;
        }

        RightClickDropDownPanelUI.Instance.Show(KeyActions, transform, null, this);
    }
}
