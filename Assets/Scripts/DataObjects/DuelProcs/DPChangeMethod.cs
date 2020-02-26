﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DPChangeMethod", menuName = "DataObjects/DuelProcs/DPChangeMethod", order = 2)]
public class DPChangeMethod : DuelProc
{
    public List<PlotMethod> ForMethodCondition;

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

        PlottingDuelUI.Instance.ChangeMethod(PlottingDuelUI.Instance.CurrentPlot.Method);
    }

    public override bool PassedConditions()
    {
        if(!base.PassedConditions())
        {
            return false;
        }
        
        if (PlottingDuelUI.Instance.ParticipantsPortraits == null || PlottingDuelUI.Instance.ParticipantsPortraits.Count == 0)
        {
            return false;
        }

        if (PlottingDuelUI.Instance.ParticipantsPortraits[0].CurrentCharacter.TopEmployer != CORE.PC)
        {
            return false;
        }

        if(ForMethodCondition != null  && ForMethodCondition.Count > 0 && ForMethodCondition.Contains(PlottingDuelUI.Instance.CurrentPlot.Method))
        {
            return false;
        }

        return true;

    }
}
