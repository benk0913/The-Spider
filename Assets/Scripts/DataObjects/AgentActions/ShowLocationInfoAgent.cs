using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShowLocationInfoAgent", menuName = "DataObjects/AgentActions/ShowLocationInfoAgent", order = 2)]
public class ShowLocationInfoAgent : AgentAction
{

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

        LocationInfoUI.Instance.Show(location);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target)
    {
        if (!base.CanDoAction(requester, character, target))
        {
            return false;
        }

        if (target == null)
        {
            return false;
        }

        return true;
    }
}
