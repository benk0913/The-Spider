using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GetArrested", menuName = "DataObjects/AgentActions/GetArrested", order = 2)]
public class GetArrested : AgentAction //DO NOT INHERIT FROM
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

        if (target.GetType() == typeof(LocationEntity))
        {
            LocationEntity targetLocation = (LocationEntity)target;

            if (!targetLocation.CurrentProperty.Traits.Contains(CORE.Instance.Database.LawAreaTrait)) //LOCATION IS NOT A CONSTABULARY / ETC..
            {
                targetLocation = CORE.Instance.FindClosestLocationWithTrait(CORE.Instance.Database.LawAreaTrait, targetLocation);
                if (targetLocation != null)
                {
                    target = targetLocation;
                }
            }
        }

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

        if (requester != CORE.Instance.Database.GOD && character.TopEmployer != requester)
        {
            return false;
        }

        return true;
    }
}
