using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BeKilled", menuName = "DataObjects/AgentActions/BeKilled", order = 2)]
public class BeKilled : AgentAction
{
    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        if (character.TopEmployer == CORE.PC)
        {
            CORE.Instance.SplineAnimationObject("BadReputationCollectedWorld",
              character.CurrentLocation.transform,
              StatsViewUI.Instance.transform,
              null,
              false);
        }

        character.TopEmployer.Reputation -= 1;
        character.Death();
    }
}
