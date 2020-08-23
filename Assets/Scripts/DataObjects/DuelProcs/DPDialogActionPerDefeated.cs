using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DPDialogActionPerDefeated", menuName = "DataObjects/DuelProcs/DPDialogActionPerDefeated", order = 2)]
public class DPDialogActionPerDefeated : DuelProc
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
            PlottingDuelUI.Instance.SetProcEventFailed(this);
            yield break;
        }

        List<Character> Defeated;

        if(isGoodForDefenders)
        {
            Defeated = PlottingDuelUI.Instance.CurrentPlot.TargetParticipants.FindAll(x =>
                PlottingDuelUI.Instance.TargetsPortraits.Find(y => y.CurrentCharacter == x) == null);//Find all which are targets but not in combat anymore.
        }
        else
        {
            Defeated = PlottingDuelUI.Instance.CurrentPlot.Participants.FindAll(x =>
                PlottingDuelUI.Instance.ParticipantsPortraits.Find(y => y.CurrentCharacter == x) == null);//Find all which are participants but not in combat anymore.
        }

        if(Defeated == null || Defeated.Count == 0)
        {
            yield break;
        }
        
        yield return PlottingDuelUI.Instance.StartCoroutine(PlottingDuelUI.Instance.SetProcEvent(this));

        Action.Activate();
    }

    public override bool PassedConditions()
    {
        if(!base.PassedConditions())
        {
            return false;
        }

        if(isGoodForDefenders)
        {
            if(PlottingDuelUI.Instance.CurrentPlot.TargetParticipants == null || PlottingDuelUI.Instance.CurrentPlot.TargetParticipants.Count == 0)
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
