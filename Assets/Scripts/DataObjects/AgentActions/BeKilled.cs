using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BeKilled", menuName = "DataObjects/AgentActions/BeKilled", order = 2)]
public class BeKilled : AgentAction
{
    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        character.Death();
    }
}
