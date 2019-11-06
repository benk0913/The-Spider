using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorkComplete", menuName = "DataObjects/AgentActions/Work/WorkComplete", order = 2)]
public class WorkComplete : AgentAction
{

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

        EarnGold(requester, character, target);
        EarnConnections(requester, character, target);
        EarnRumors(requester, character, target);
    }

    public virtual void EarnConnections(Character requester, Character character, AgentInteractable target, int addedConnections = 0)
    {
        if (character.WorkLocation == null)
        {
            return;
        }

        float earnedConnections =
            character.WorkLocation.CurrentAction.ConnectionsGenerated
            * CORE.Instance.Database.Stats.GlobalRevenueMultiplier;

        if (Mathf.RoundToInt(earnedConnections) > 0)
        {
            if(character.TopEmployer == null)
            {
                character.Connections += Mathf.RoundToInt(earnedConnections + addedConnections);
                return;
            }

            if (character.TopEmployer == CORE.PC)
            {
                CORE.Instance.SplineAnimationObject(
                    prefabKey: "ConnectionCollectedWorld",
                    startPoint: character.WorkLocation.transform,
                    targetPoint: StatsViewUI.Instance.GoldText.transform,
                    ()=> { StatsViewUI.Instance.RefreshConnections(); },
                    canvasElement: false);
            }

            character.TopEmployer.Connections += Mathf.RoundToInt(earnedConnections + addedConnections);
        }
    }

    public virtual void EarnRumors(Character requester, Character character, AgentInteractable target, int addedRumors = 0)
    {
        if (character.WorkLocation == null)
        {
            return;
        }

        float earnedRumors =
            character.WorkLocation.CurrentAction.RumorsGenerated
            * CORE.Instance.Database.Stats.GlobalRevenueMultiplier;

        if (Mathf.RoundToInt(earnedRumors) > 0)
        {

            if(character.TopEmployer == null)
            {
                character.Rumors += Mathf.RoundToInt(earnedRumors + addedRumors);
                return;
            }

            if (character.TopEmployer == CORE.PC)
            {
                CORE.Instance.SplineAnimationObject(
                    prefabKey: "EarCollectedWorld",
                    startPoint: character.WorkLocation.transform,
                    targetPoint: StatsViewUI.Instance.GoldText.transform,
                    () => { StatsViewUI.Instance.RefreshRumors(); },
                    canvasElement: false);
            }

            character.TopEmployer.Rumors += Mathf.RoundToInt(earnedRumors + addedRumors);
        }
    }

    public virtual void EarnGold(Character requester, Character character, AgentInteractable target, int addedGold = 0) 
    {
        if(character.WorkLocation == null)
        {
            return;
        }

        float earnedGold =
            character.WorkLocation.CurrentAction.GoldGenerated
            * CORE.Instance.Database.Stats.GlobalRevenueMultiplier;

        if (Mathf.RoundToInt(earnedGold) > 0)
        {
            if(character.TopEmployer == null)
            {
                character.TopEmployer.Gold += Mathf.RoundToInt(earnedGold + addedGold);
                return;
            }

            if (character.TopEmployer == CORE.PC)
            {
                CORE.Instance.SplineAnimationObject(
                    prefabKey: "CoinCollectedWorld",
                    startPoint: character.WorkLocation.transform,
                    targetPoint: StatsViewUI.Instance.GoldText.transform,
                    () => { StatsViewUI.Instance.RefreshGold(); },
                    canvasElement: false);
            }

            character.TopEmployer.Gold += Mathf.RoundToInt(earnedGold + addedGold);
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
