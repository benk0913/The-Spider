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
            PlottingDuelUI.Instance.SetProcEventFailed(this);
            yield break;
        }

        yield return PlottingDuelUI.Instance.StartCoroutine(PlottingDuelUI.Instance.SetProcEvent(this));

        Character victim = null;
        PortraitUI victimPortrait = null;

        if (isGoodForDefenders)
        {
            if (PlottingDuelUI.Instance.ParticipantsPortraits.Count == 0)
            {
                yield break;
            }

            if (IsRandom)
            {
                victimPortrait = PlottingDuelUI.Instance.ParticipantsPortraits[Random.Range(0, PlottingDuelUI.Instance.ParticipantsPortraits.Count)];
                
            }
            else
            {
                victimPortrait = PlottingDuelUI.Instance.ParticipantsPortraits[0];
                victim = victimPortrait.CurrentCharacter;
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
                victimPortrait = PlottingDuelUI.Instance.TargetsPortraits[Random.Range(0, PlottingDuelUI.Instance.TargetsPortraits.Count)];
                victim = victimPortrait.CurrentCharacter;
            }
            else
            {
                victimPortrait = PlottingDuelUI.Instance.TargetsPortraits[0];
                victim = victimPortrait.CurrentCharacter;
            }
        }

        GenerateEffectOnPortrait(victimPortrait);
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
