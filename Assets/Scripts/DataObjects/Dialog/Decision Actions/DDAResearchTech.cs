using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAResearchTech", menuName = "DataObjects/Dialog/Actions/DDAResearchTech", order = 2)]
public class DDAResearchTech : DialogDecisionAction
{
    [SerializeField]
    TechTreeItem Tech;

    [SerializeField]
    bool ResearchState = true;

    public override void Activate()
    {
        TechTreeItem originalTech = CORE.Instance.TechTree.Find(x => x.name == Tech.name);

        if(originalTech == null)
        {
            Debug.LogError("Can't find Tech " + Tech.name);
            return;
        }

        originalTech.IsResearched = ResearchState;
    }
}
