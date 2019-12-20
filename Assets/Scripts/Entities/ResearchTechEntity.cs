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

    private void Start()
    {
        if(RequiredTech == null)
        {
            return;
        }

        CORE.Instance.SubscribeToEvent("ResearchComplete", OnResearchComplete);
    }

    private void OnResearchComplete()
    {
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
