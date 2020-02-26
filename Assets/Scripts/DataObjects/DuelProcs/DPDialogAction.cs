using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DPDialogAction", menuName = "DataObjects/DuelProcs/DPDialogAction", order = 2)]
public class DPDialogAction : DuelProc
{
    public DialogDecisionAction Action;

    public override IEnumerator Execute()
    {
        if(!PassedConditions())
        {
            yield break;
        }

        if (!RollChance())
        {
            yield break;
        }

        yield return PlottingDuelUI.Instance.StartCoroutine(PlottingDuelUI.Instance.SetProcEvent(this));

        Action.Activate();
    }
}
