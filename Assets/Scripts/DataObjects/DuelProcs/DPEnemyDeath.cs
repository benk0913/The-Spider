using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DPEnemyDeath", menuName = "DataObjects/DuelProcs/DPEnemyDeath", order = 2)]
public class DPEnemyDeath : DuelProc
{
    public override void Execute()
    {
        if(PlottingDuelUI.Instance.TargetsPortraits == null || PlottingDuelUI.Instance.TargetsPortraits.Count == 0)
        {
            return;
        }

        if(PlottingDuelUI.Instance.TargetsPortraits[0].CurrentCharacter.TopEmployer != CORE.PC)
        {
            return;
        }

        foreach(PortraitUI portrait in PlottingDuelUI.Instance.ParticipantsPortraits)
        {
            if(!RollChance())
            {
                continue;
            }

            PlottingDuelUI.Instance.KillCharacter(portrait.CurrentCharacter);
        }
    }
}
