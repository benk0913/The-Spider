using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DPTakeOverOpponent", menuName = "DataObjects/DuelProcs/DPTakeOverOpponent", order = 2)]
public class DPTakeOverOpponent : DuelProc
{
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
        PortraitUI victimPortrait;
        if (isGoodForDefenders)//Get victim of opposite team.
        {
            victim = PlottingDuelUI.Instance.CurrentPlot.Participants[Random.Range(0, PlottingDuelUI.Instance.CurrentPlot.Participants.Count)];
            PlottingDuelUI.Instance.KillCharacter(victim);
            victimPortrait = PlottingDuelUI.Instance.GenerateTarget(victim);
        }
        else
        {
            victim = PlottingDuelUI.Instance.CurrentPlot.TargetParticipants[Random.Range(0, PlottingDuelUI.Instance.CurrentPlot.TargetParticipants.Count)];
            PlottingDuelUI.Instance.KillCharacter(victim);
            victimPortrait = PlottingDuelUI.Instance.GenerateParticipant(victim);
        }

        GenerateEffectOnPortrait(victimPortrait);
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
