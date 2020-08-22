using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResearchTechEntity : MonoBehaviour
{
    [SerializeField]
    TechTreeItem RequiredTech;

    [SerializeField]
    UnityEvent OnResearch;

    [SerializeField]
    UnityEvent Reset;

    private void Start()
    {
        if(RequiredTech == null)
        {
            return;
        }

        CORE.Instance.SubscribeToEvent("ResearchComplete", OnResearchComplete);
        CORE.Instance.SubscribeToEvent("GameLoadComplete", OnResearchComplete);
        OnResearchComplete();
    }

    private void OnEnable()
    {

        if (CORE.Instance == null || CORE.Instance.TechTree == null)
        {
            Reset.Invoke();
            return;
        }

        if (!CORE.Instance.TechTree.Find(x => x.name == RequiredTech.name).IsResearched)
        {
            Reset.Invoke();
        }
    }

    private void OnResearchComplete()
    {

        if (CORE.Instance.TechTree == null)
        {
            return;
        }

        if(CORE.Instance.TechTree.Find(x => x.name == RequiredTech.name).IsResearched)
        {
            OnResearch.Invoke();
            StopListening();
        }
    }

    public void StopListening()
    {
        CORE.Instance.UnsubscribeFromEvent("ResearchComplete", OnResearchComplete);
    }
}
