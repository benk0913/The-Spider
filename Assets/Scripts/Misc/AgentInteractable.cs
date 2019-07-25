using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentInteractable : MonoBehaviour
{
    public virtual List<AgentAction> GetPossibleActions(Character forCharacter)
    {
        return null;
    }

    public void ShowActionMenu()
    {
        if (ControlCharacterPanelUI.CurrentCharacter == null)
        {
            return;
        }

        List<AgentAction> currentActions = GetPossibleActions(ControlCharacterPanelUI.CurrentCharacter);
        List<DescribedAction> KeyActions = new List<DescribedAction>();

        foreach (AgentAction action in currentActions)
        {
            if (!action.CanDoAction(ControlCharacterPanelUI.CurrentCharacter, this))
            {
                continue;
            }

            KeyActions.Add(
                new DescribedAction(
                    action.name,
                    () => action.Execute(ControlCharacterPanelUI.CurrentCharacter, this)
                    , action.Description));
        }

        if (KeyActions.Count == 0)
        {
            return;
        }

        RightClickDropDownPanelUI.Instance.Show(KeyActions, transform);
    }
}
