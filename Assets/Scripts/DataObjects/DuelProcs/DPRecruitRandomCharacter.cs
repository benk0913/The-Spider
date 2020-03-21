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
}
