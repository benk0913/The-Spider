using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DPDeath", menuName = "DataObjects/DuelProcs/DPDeath", order = 2)]
public class DPDeath : DuelProc
{

    public override IEnumerator Execute()
    {
        if(!PassedConditions())
        {
            yield break;
        }

        yield return PlottingDuelUI.Instance.StartCoroutine(PlottingDuelUI.Instance.SetProcEvent(this));

        List<PortraitUI> affected;

        if (isGoodForDefenders)
        {
            affected = PlottingDuelUI.Instance.ParticipantsPortraits;
        }
        else
        {
            affected = PlottingDuelUI.Instance.TargetsPortraits;
        }


        List<PortraitUI> dead = new List<PortraitUI>();
        foreach (PortraitUI portrait in affected)
        {
            if(!RollChance())
            {
                yield break;
            }

            dead.Add(portrait);
        }

        while(dead.Count > 0)
        {
            PlottingDuelUI.Instance.KillCharacter(dead[0].CurrentCharacter);
            dead.RemoveAt(0);
        }
    }


    public override bool PassedConditions()
    {
        if (!base.PassedConditions())
        {
            return false;
        }

        if (isGoodForDefenders)
        {
            if (PlottingDuelUI.Instance.TargetsPortraits == null || PlottingDuelUI.Instance.TargetsPortraits.Count == 0)
            {
                return false;
            }

            if (PlottingDuelUI.Instance.TargetsPortraits[0].CurrentCharacter.TopEmployer != CORE.PC)
            {
                return false;
            }
        }
        else
        {
            if (PlottingDuelUI.Instance.ParticipantsPortraits == null || PlottingDuelUI.Instance.ParticipantsPortraits.Count == 0)
            {
                return false;
            }

            if (PlottingDuelUI.Instance.ParticipantsPortraits[0].CurrentCharacter.TopEmployer != CORE.PC)
            {
                return false;
            }
        }

        return true;
    }
}
