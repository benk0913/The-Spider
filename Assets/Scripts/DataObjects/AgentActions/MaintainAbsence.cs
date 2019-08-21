using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MaintainAbsence", menuName = "DataObjects/AgentActions/MaintainAbsence", order = 2)]
public class MaintainAbsence : AgentAction //DO NOT INHERIT FROM
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

        target = CORE.Instance.GetRandomLocationWithTrait(CORE.Instance.Database.WildernessAreaTrait);

        LongTermTaskEntity longTermTask = ResourcesLoader.Instance.GetRecycledObject("LongTermTaskEntity").GetComponent<LongTermTaskEntity>();

        longTermTask.transform.SetParent(MapViewManager.Instance.transform);
        longTermTask.transform.position = target.transform.position;
        longTermTask.SetInfo(this.Task, requester, character, target);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target)
    {
        LocationEntity location = (LocationEntity)target;

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
