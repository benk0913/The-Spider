using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorkComplete", menuName = "DataObjects/AgentActions/Work/WorkComplete", order = 2)]
public class WorkComplete : AgentAction
{

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        string reason;
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

        EarnGold(requester, character, target);
    }

    public virtual void EarnGold(Character requester, Character character, AgentInteractable target)
    {
        if(character.WorkLocation == null)
        {
            return;
        }

        BonusChallenge workChallenge = character.WorkLocation.CurrentAction.WorkAction.Challenge;

        float characterBonusValue = character.GetBonus(workChallenge.Type).Value; // character natural skill

        if(target.GetType() == typeof(LocationEntity)) // location effects on skill
        {
            LocationEntity location = (LocationEntity)target;
            foreach(PropertyTrait trait in location.Traits)
            {
                foreach(Bonus bonus in trait.Bonuses)
                {
                    if(bonus.Type == workChallenge.Type)
                    {
                        characterBonusValue += bonus.Value;
                    }
                }
            }
        }

        float earnedSum =
            Random.Range(character.WorkLocation.CurrentAction.GoldGeneratedMin, character.WorkLocation.CurrentAction.GoldGeneratedMax)
            * CORE.Instance.Database.Stats.GlobalRevenueMultiplier
            * (workChallenge != null? (characterBonusValue / (float)workChallenge.ChallengeValue) : 1);

        if (character.WorkLocation.OwnerCharacter != null && character.WorkLocation.CurrentProperty.ManagementBonus != null)
        {
            float bossMulti = character.WorkLocation.OwnerCharacter.GetBonus(character.WorkLocation.CurrentProperty.ManagementBonus).Value;
            earnedSum *= bossMulti;
        }

        if (Mathf.RoundToInt(earnedSum) > 0)
        {
            if (character.TopEmployer == CORE.PC)
            {
                CORE.Instance.SplineAnimationObject(
                    prefabKey: "CoinCollectedWorld",
                    startPoint: character.WorkLocation.transform,
                    targetPoint: StatsViewUI.Instance.GoldText.transform,
                    OnComplete: () =>
                    {
                        character.Gold += Mathf.RoundToInt(earnedSum);
                    },
                    canvasElement: false);
            }
            else
            {
                character.Gold += Mathf.RoundToInt(earnedSum);
            }
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out string reason)
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
