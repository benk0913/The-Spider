using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DPDeathSingle", menuName = "DataObjects/DuelProcs/DPDeathSingle", order = 2)]
public class DPDeathSingle : DuelProc
{
    public bool IsRandom;

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

        Character victim;

        if (isGoodForDefenders)
        {
            if (PlottingDuelUI.Instance.ParticipantsPortraits.Count == 0)
            {
                yield break;
            }

            if (IsRandom)
            {
                victim = PlottingDuelUI.Instance.ParticipantsPortraits[Random.Range(0, PlottingDuelUI.Instance.ParticipantsPortraits.Count)].CurrentCharacter;
            }
            else
            {
                victim = PlottingDuelUI.Instance.ParticipantsPortraits[0].CurrentCharacter;
            }
        }
        else
        {
            if (PlottingDuelUI.Instance.TargetsPortraits.Count == 0)
            {
                yield break;
            }

            if (IsRandom)
            {
                victim = PlottingDuelUI.Instance.TargetsPortraits[Random.Range(0, PlottingDuelUI.Instance.TargetsPortraits.Count)].CurrentCharacter;
            }
            else
            {
                victim = PlottingDuelUI.Instance.TargetsPortraits[0].CurrentCharacter;
            }
        }

        PlottingDuelUI.Instance.KillCharacter(victim);
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
