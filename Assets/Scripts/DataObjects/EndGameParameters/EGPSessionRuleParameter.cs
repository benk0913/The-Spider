using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EGPSessionRuleParameter", menuName = "DataObjects/EndGameParameter/EGPSessionRuleParameter", order = 2)]
public class EGPSessionRuleParameter : EndGameParameter
{
    public List<EndgameParamInstanceSessionRule> Instances = new List<EndgameParamInstanceSessionRule>();
    public EndgameParamInstance DefaultInstance;

    public override string GetValue()
    {
        foreach(EndgameParamInstanceSessionRule instance in Instances)
        {
            if(CORE.Instance.SessionRules.Rules.Find(x=> x.name == instance.Rule.name) != null)
            {
                return instance.Key;
            }
        }

        return DefaultInstance.Key;
    }

    public override Sprite GetIcon()
    {
        foreach (EndgameParamInstanceSessionRule instance in Instances)
        {
            if (CORE.Instance.SessionRules.Rules.Find(x => x.name == instance.Rule.name) != null)
            {
                return instance.Icon;
            }
        }

        return DefaultInstance.Icon;
    }

    [System.Serializable]
    public class EndgameParamInstanceSessionRule : EndgameParamInstance
    {
        public SessionRule Rule;
    }
}
