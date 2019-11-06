using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StreetPatrolComplete", menuName = "DataObjects/AgentActions/Work/StreetPatrolComplete", order = 2)]
public class StreetPatrolComplete : AgentAction
{

    public int DuelBonus;

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            return;
        }

        if (!RollSucceed(character))
        {
            if (FailureResult != null)
            {
                FailureResult.Execute(requester, character, target);
            }

            return;
        }

        LocationEntity location = (LocationEntity)target;

        List<Character> illegalTaskChars = location.CharactersInLocation.FindAll(
            (Character charInQuestion) => 
            {
                return charInQuestion.CurrentTaskEntity != null && charInQuestion.CurrentTaskEntity.CurrentTask.Illegal;
            });

        if(illegalTaskChars == null || illegalTaskChars.Count == 0)
        {
            return;
        }

        float targetDiscreet;
        float heroAware;
        foreach(Character targetChar in illegalTaskChars)
        {
            targetDiscreet = targetChar.GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value;
            heroAware = targetChar.GetBonus(CORE.Instance.Database.GetBonusType("Aware")).Value;
            if (Random.Range(0,targetDiscreet+heroAware+DuelBonus) < heroAware+DuelBonus)
            {
                targetChar.StopDoingCurrentTask();
                CORE.Instance.Database.GetEventAction("Get Arrested").Execute(CORE.Instance.Database.GOD, targetChar, target);
                return;
            }
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (requester != CORE.Instance.Database.GOD && character.TopEmployer != requester && requester != character)
        {
            return false;
        }

        return true;
    }
}
