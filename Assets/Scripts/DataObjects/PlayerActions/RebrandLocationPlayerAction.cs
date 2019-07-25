using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RebrandLocationPlayerAction", menuName = "DataObjects/PlayerActions/RebrandLocationPlayerAction", order = 2)]
public class RebrandLocationPlayerAction : PlayerAction
{
    [TextArea(2, 3)]
    public string Description;

    public override void Execute(AgentInteractable target)
    {
        if (!CanDoAction(target))
        {
            GlobalMessagePrompterUI.Instance.Show("You cannot change this property.", 1f, Color.yellow);
            return;
        }

        LocationEntity location = (LocationEntity)target;

        RebrandWindowUI.Instance.Show(location);
    }

    public override bool CanDoAction(AgentInteractable target)
    {
        LocationEntity location = (LocationEntity)target;

        if (location.CurrentProperty.PlotType.name == DEF.UNIQUE_PLOT)
        {
            return false;
        }

        return true;
    }
}
