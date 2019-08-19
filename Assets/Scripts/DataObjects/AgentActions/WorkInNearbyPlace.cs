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

        if (!CanDoAction(requester, character, target))
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
        target = CORE.Instance.FindClosestLocationWithTrait(CORE.Instance.Database.PublicAreaTrait, location);

        LongTermTaskEntity longTermTask = ResourcesLoader.Instance.GetRecycledObject("LongTermTaskEntity").GetComponent<LongTermTaskEntity>();

        longTermTask.transform.SetParent(MapViewManager.Instance.transform);
        longTermTask.transform.position = target.transform.position;
        longTermTask.SetInfo(this.Task, requester, character, target);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target)
    {

        if (!base.CanDoAction(requester, character, target))
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
