using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WorkInNearbyPlace", menuName = "DataObjects/AgentActions/Work/WorkInNearbyPlace", order = 2)]
public class WorkInNearbyPlace : AgentAction //DO NOT INHERIT FROM
{
    public LongTermTask Task;

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            return;
        }
        
        if (!RollSucceed(character))
        {
            if (FailureResult != null)
            {
                FailureResult.Execute(requester, character, target);
            }

            return;
        }

        LocationEntity location = (LocationEntity)target;

        //FIND Closest public area
        if (Random.Range(0, 1f) > 0.2f)
        {
            target = CORE.Instance.GetClosestLocationWithTrait(CORE.Instance.Database.PublicAreaTrait, location);
        }
        else
        {
            target = CORE.Instance.GetRandomLocationWithTrait(CORE.Instance.Database.PublicAreaTrait);
        }

        CORE.Instance.GenerateLongTermTask(this.Task, requester, character, (LocationEntity)target, null, -1, null, this);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (requester != character && requester != CORE.Instance.Database.GOD && character.TopEmployer != requester)
        {
            return false;
        }

        return true;
    }
}
