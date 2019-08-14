using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RebrandLocationPlayerAction", menuName = "DataObjects/PlayerActions/RebrandLocationPlayerAction", order = 2)]
public class RebrandLocationPlayerAction : PlayerAction
{
    [TextArea(2, 3)]
    public string Description;

    public override void Execute(Character requester, AgentInteractable target)
    {
        if (!CanDoAction(requester, target))
        {
            GlobalMessagePrompterUI.Instance.Show("You cannot change this property.", 1f, Color.yellow);
            return;
        }

        LocationEntity location = (LocationEntity)target;

        RebrandWindowUI.Instance.Show(location);
    }

    public override bool CanDoAction(Character requester, AgentInteractable target)
    {
        LocationEntity location = (LocationEntity)target;

        if (location.CurrentProperty.PlotType == CORE.Instance.Database.UniquePlotType)
        {
            return false;
        }

        return true;
    }
}
