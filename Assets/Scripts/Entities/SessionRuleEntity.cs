using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SessionRuleEntity : MonoBehaviour
{
    [SerializeField]
    SessionRule RequiresRule;

    [SerializeField]
    UnityEvent OnGainRule;

    [SerializeField]
    UnityEvent Reset;

    private void Start()
    {
        if(RequiresRule == null)
        {
            return;
        }

        CORE.Instance.SubscribeToEvent("GainSessionRule", OnHaveRule);
        CORE.Instance.SubscribeToEvent("GameLoadComplete", OnHaveRule);
        OnHaveRule();
    }

    private void OnEnable()
    {
        OnHaveRule();
    }

    private void OnHaveRule()
    {
        if(CORE.Instance.SessionRules == null)
        {
            Reset.Invoke();
            return;
        }

        if(CORE.Instance.SessionRules.Rules.Find(x => x.name == RequiresRule.name) == null)
        {
            Reset.Invoke();
            return;
        }

        OnGainRule.Invoke();
        StopListening();
    }

    public void StopListening()
    {
        CORE.Instance.UnsubscribeFromEvent("GainSessionRule", OnHaveRule);
    }
}
