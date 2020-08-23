using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DPRecruitRandomCharacter", menuName = "DataObjects/DuelProcs/DPRecruitRandomCharacter", order = 2)]
public class DPRecruitRandomCharacter : DuelProc
{
    public int Count = 1;

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
        

        if (isGoodForDefenders)
        {
            for (int i = 0; i < Count; i++)
            {
                Character character = CORE.Instance.GenerateCharacter(-1, 16, 90, null);
                character.Randomize();
                PortraitUI portrait = PlottingDuelUI.Instance.GenerateTarget(character);
                GenerateEffectOnPortrait(portrait);
            }
        }
        else
        {
            for (int i = 0; i < Count; i++)
            {
                Character character = CORE.Instance.GenerateCharacter(-1, 16, 90, null);
                character.Randomize();
                PortraitUI portrait = PlottingDuelUI.Instance.GenerateParticipant(character);
                GenerateEffectOnPortrait(portrait);
            }
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
