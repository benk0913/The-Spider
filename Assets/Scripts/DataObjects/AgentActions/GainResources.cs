using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GainResources", menuName = "DataObjects/AgentActions/GainResources", order = 2)]
public class GainResources : AgentAction //DO NOT INHERIT FROM
{
    public int Gold;
    public int Connections;
    public int Rumors;
    public int Progression;
    public int Reputation;
    public List<Item> Items = new List<Item>();

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
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

        base.Execute(requester, character, target);

        requester.Gold        += this.Gold;
        requester.Connections += this.Connections;
        requester.Rumors      += this.Rumors;
        requester.Progress    += this.Progression;
        requester.Reputation  += this.Reputation;
        Items.ForEach((x) => requester.Belogings.Add(x.Clone()));
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        return true;
    }
}
