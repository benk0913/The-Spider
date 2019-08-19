﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorkComplete", menuName = "DataObjects/AgentActions/Work/WorkComplete", order = 2)]
public class WorkComplete : AgentAction
{
    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        if (!CanDoAction(requester, character, target))
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

        EarnGold(requester, character, target);
    }

    public virtual void EarnGold(Character requester, Character character, AgentInteractable target)
    {
        if(character.WorkLocation == null)
        {
            return;
        }

        BonusChallenge workChallenge = character.WorkLocation.CurrentAction.WorkAction.Challenge;

        float earnedSum =
            Random.Range(character.WorkLocation.CurrentAction.GoldGeneratedMin, character.WorkLocation.CurrentAction.GoldGeneratedMax)
            * CORE.Instance.Database.Stats.GlobalRevenueMultiplier
            * (workChallenge != null? ((float)character.GetBonus(workChallenge.Type).Value / (float)workChallenge.ChallengeValue) : 1);

        if (character.WorkLocation.OwnerCharacter != null && character.WorkLocation.CurrentProperty.ManagementBonus != null)
        {
            float bossMulti = character.WorkLocation.OwnerCharacter.GetBonus(character.WorkLocation.CurrentProperty.ManagementBonus).Value;
            earnedSum *= bossMulti;
        }

        character.Gold += Mathf.RoundToInt(earnedSum);
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target)
    {
        if (!base.CanDoAction(requester, character, target))
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
