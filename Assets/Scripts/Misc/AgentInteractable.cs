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

        foreach (AgentAction action in currentActions)
        {

            KeyActions.Add(
                new DescribedAction(
                    action.name,
                    () => action.Execute(ControlCharacterPanelUI.CurrentCharacter, this)
                    , action.Description
                    , action.CanDoAction(ControlCharacterPanelUI.CurrentCharacter, this)));
        }

        if (KeyActions.Count == 0)
        {
            return;
        }

        RightClickDropDownPanelUI.Instance.Show(KeyActions, transform, ControlCharacterPanelUI.CurrentCharacter);
    }

    public void ShowPlayerActionMenu()
    {

        List<PlayerAction> currentActions = GetPossiblePlayerActions();
        List<DescribedAction> KeyActions = new List<DescribedAction>();

        foreach (PlayerAction action in currentActions)
        {
            KeyActions.Add(
                new DescribedAction(
                    action.name,
                    () => action.Execute(this)
                    , action.Description
                    , action.CanDoAction(this)));
        }

        if (KeyActions.Count == 0)
        {
            return;
        }

        RightClickDropDownPanelUI.Instance.Show(KeyActions, transform, null);
    }
}
