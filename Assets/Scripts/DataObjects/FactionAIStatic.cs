using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "FactionAIStatic", menuName = "DataObjects/FactionAIStatic", order = 2)]
public class FactionAIStatic : FactionAI
{
    public override void Expand()
    {
        AttemptMaximizeEmployees();

        AttemptMaximizeGuards();

        AttemptMaintainance();
    }

    public override void Agression()
    {
        return;
    }

}
