using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "UpgradeLocationPlayerAction", menuName = "DataObjects/PlayerActions/UpgradeLocationPlayerAction", order = 2)]
public class UpgradeLocationPlayerAction : PlayerAction
{
    [TextArea(2, 3)]
    public string Description;

    public override void Execute(AgentInteractable target)
    {
        if (!CanDoAction(target))
        {
            GlobalMessagePrompterUI.Instance.Show("You cannot upgrade this property yet.", 1f, Color.yellow);
            return;
        }

        LocationEntity location = (LocationEntity)target;

        location.PurchaseUpgrade();
    }

    public override bool CanDoAction(AgentInteractable target)
    {
        LocationEntity location = (LocationEntity)target;

        if(location.IsUpgrading)
        {
            return false;
        }

        if (location.CurrentProperty.PropertyLevels.Count == location.Level)
        {
            return false;
        }

        if (location.CurrentProperty.PropertyLevels[location.Level - 1].UpgradePrice > CORE.PC.Gold)
        {
            return false;
        }

        return true;
    }
}
