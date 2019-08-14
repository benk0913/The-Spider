using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "UpgradeLocationPlayerAction", menuName = "DataObjects/PlayerActions/UpgradeLocationPlayerAction", order = 2)]
public class UpgradeLocationPlayerAction : PlayerAction
{

    public override void Execute(Character requester, AgentInteractable target)
    {
        if (!CanDoAction(requester, target))
        {
            GlobalMessagePrompterUI.Instance.Show("You cannot upgrade this property yet.", 1f, Color.yellow);
            return;
        }

        LocationEntity location = (LocationEntity)target;
        
        location.PurchaseUpgrade(requester);
    }

    public override bool CanDoAction(Character requester, AgentInteractable target)
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

        if (location.CurrentProperty.PropertyLevels[location.Level - 1].UpgradePrice > requester.Gold)
        {
            return false;
        }

        return true;
    }
}
