using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DPRessurectAlly", menuName = "DataObjects/DuelProcs/DPRessurectAlly", order = 2)]
public class DPRessurectAlly : DuelProc
{
    public bool isRandom;
    
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

        Character ressurrected;

        if (isGoodForDefenders)
        {
            if (isRandom || PlottingDuelUI.Instance.LastDefeatedTarget == null)
            {
                ressurrected = PlottingDuelUI.Instance.CurrentPlot.TargetParticipants[Random.Range(0, PlottingDuelUI.Instance.CurrentPlot.TargetParticipants.Count)];
            }
            else
            {
                ressurrected = PlottingDuelUI.Instance.LastDefeatedTarget;
            }

            PlottingDuelUI.Instance.GenerateTarget(ressurrected);
        }
        else
        {
            if (isRandom || PlottingDuelUI.Instance.LastDefeatedParticipant == null)
            {
                ressurrected = PlottingDuelUI.Instance.CurrentPlot.Participants[Random.Range(0, PlottingDuelUI.Instance.CurrentPlot.Participants.Count)];
            }
            else
            {
                ressurrected = PlottingDuelUI.Instance.LastDefeatedParticipant;
            }

            PlottingDuelUI.Instance.GenerateParticipant(ressurrected);
        }
    }

    public override bool PassedConditions()
    {
        if(!base.PassedConditions())
        {
            return false;
        }

        if (isGoodForDefenders)
        {
            if (PlottingDuelUI.Instance.CurrentPlot.TargetParticipants == null || PlottingDuelUI.Instance.CurrentPlot.TargetParticipants.Count == 0)
            {
                return false;
            }

            if (PlottingDuelUI.Instance.CurrentPlot.TargetParticipants[0].TopEmployer != CORE.PC)
            {
                return false;
            }
        }
        else
        {
            if (PlottingDuelUI.Instance.CurrentPlot.Participants == null || PlottingDuelUI.Instance.CurrentPlot.Participants.Count == 0)
            {
                return false;
            }

            if (PlottingDuelUI.Instance.CurrentPlot.Participants[0].TopEmployer != CORE.PC)
            {
                return false;
            }
        }


        return true;

    }
}
