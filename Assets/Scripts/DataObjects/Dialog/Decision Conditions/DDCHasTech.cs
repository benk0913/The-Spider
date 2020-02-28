using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDCHasTech", menuName = "DataObjects/Dialog/Conditions/DDCHasTech", order = 2)]
public class DDCHasTech : DialogDecisionCondition
{
    public TechTreeItem TheTech;

    public override bool CheckCondition()
    {
        TechTreeItem techInstance = CORE.Instance.TechTree.Find(x => x.name == TheTech.name);

        if (techInstance == null || (techInstance != null && !techInstance.IsResearched))
        {
            if (Inverted)
            {
                return base.CheckCondition();
            }

            return false;
        }

        return base.CheckCondition();
    }
}
