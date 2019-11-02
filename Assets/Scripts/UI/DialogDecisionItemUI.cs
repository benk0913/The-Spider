
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogDecisionItemUI : MonoBehaviour
{
    [SerializeField]
    Image Icon;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    [SerializeField]
    Button ClickableButton;

    DialogDecision CurrentDecision;

    public void SetInfo(DialogDecision decision)
    {
        CurrentDecision = decision;

        RefreshUI();
    }

    void RefreshUI()
    {
        Icon.sprite = CurrentDecision.Icon;
        TooltipTarget.SetTooltip(CurrentDecision.Title);

        ClickableButton.interactable = true;

        foreach (DialogDecisionCondition condition in CurrentDecision.ActiveConditions)
        {
            if(!condition.CheckCondition())
            {
                if (ClickableButton.interactable)
                {
                    ClickableButton.interactable = false;
                    break;
                }
            }
        }

        if(!ClickableButton.interactable)
        {
            TooltipTarget.Text += "\n <color=yellow><u> Requires Conditions: </u>";

            foreach (DialogDecisionCondition condition in CurrentDecision.ActiveConditions)
            {
                TooltipTarget.Text += "\n "+  (condition.CheckCondition() ? "<color=green>" : "<color=red>") + condition.name + "</color>";
            }

            TooltipTarget.Text += "</color>";
        }



    }

    public void ActivateDecision()
    {
        CurrentDecision.Activate();
    }
}
