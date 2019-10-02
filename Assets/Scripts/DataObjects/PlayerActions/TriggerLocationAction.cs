using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TriggerLocationAction", menuName = "DataObjects/PlayerActions/TriggerLocationAction", order = 2)]
public class TriggerLocationAction : PlayerAction
{
    [SerializeField]
    LongTermTask OwnerTask;

    [SerializeField]
    PropertyTrait TargetPropertyTrait;

    public override void Execute(Character requester, AgentInteractable target)
    {
        string reason;
        if (!CanDoAction(requester, target, out reason))
        {
            GlobalMessagePrompterUI.Instance.Show("You cannot change this property.", 1f, Color.yellow);
            return;
        }

        LocationEntity location = (LocationEntity)target;
        Character locationOwner = location.OwnerCharacter;

        if(TargetPropertyTrait != null)
        {
            location = CORE.Instance.GetClosestLocationWithTrait(TargetPropertyTrait, location);
        }
        
        CORE.Instance.GenerateLongTermTask(this.OwnerTask, requester, locationOwner, location);
    }

    public override bool CanDoAction(Character requester, AgentInteractable target, out string reason)
    {
        reason = "";
        LocationEntity location = (LocationEntity)target;

        if (location.OwnerCharacter == null)
        {
            return false;
        }

        if (location.OwnerCharacter.TopEmployer != CORE.PC)
        {
            return false;
        }

        if (location.OwnerCharacter.CurrentTaskEntity != null && !location.OwnerCharacter.CurrentTaskEntity.CurrentTask.Cancelable)
        {
            return false;
        }

        return true;
    }
}
