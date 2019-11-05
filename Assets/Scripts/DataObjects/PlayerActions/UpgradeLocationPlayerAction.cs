using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "UpgradeLocationPlayerAction", menuName = "DataObjects/PlayerActions/UpgradeLocationPlayerAction", order = 2)]
public class UpgradeLocationPlayerAction : PlayerAction
{

    public override void Execute(Character requester, AgentInteractable target)
    {
        string reason;
        if (!CanDoAction(requester, target, out reason))
        {
            GlobalMessagePrompterUI.Instance.Show("You cannot upgrade this property yet.", 1f, Color.yellow);
            return;
        }

        LocationEntity location = (LocationEntity)target;
        
        location.PurchaseUpgrade(requester);
    }

    public override bool CanDoAction(Character requester, AgentInteractable target, out string reason)
    {
        reason = "";
        LocationEntity location = (LocationEntity)target;

        if(location.OwnerCharacter == null || location.OwnerCharacter.TopEmployer != requester)
        {
            return false;
        }

        if(location.IsUpgrading)
        {
            reason = location.CurrentProperty.name+" is already upgrading.";
            return false;
        }

        if (location.CurrentProperty.PropertyLevels.Count == location.Level)
        {
            reason = location.CurrentProperty.name + " has reached the highest level.";
            return false;
        }

        if (location.CurrentProperty.PropertyLevels[location.Level - 1].UpgradePrice > requester.Gold)
        {
            reason = "Requires more: " + (location.CurrentProperty.PropertyLevels[location.Level - 1].UpgradePrice - requester.Gold) + " gold.";
            return false;
        }

        return true;
    }
}
