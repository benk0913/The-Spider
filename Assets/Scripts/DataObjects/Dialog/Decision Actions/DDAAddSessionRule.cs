using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAAddSessionRule", menuName = "DataObjects/Dialog/Actions/DDAAddSessionRule", order = 2)]
public class DDAAddSessionRule : DialogDecisionAction
{
    public SessionRule Rule;
    public bool Remove = false;

    public override void Activate()
    {
        if (Remove)
        {
            CORE.Instance.SessionRules.Rules.Remove(CORE.Instance.SessionRules.Rules.Find(x => x.name == Rule.name));
            CORE.Instance.InvokeEvent("GainSessionRule");
        }
        else
        {
            if(CORE.Instance.SessionRules.Rules.Find(x=>x.name == Rule.name) != null)
            {
                return;
            }

            CORE.Instance.SessionRules.Rules.Add(Rule.Clone());
            CORE.Instance.InvokeEvent("GainSessionRule");
        }
    }
}
