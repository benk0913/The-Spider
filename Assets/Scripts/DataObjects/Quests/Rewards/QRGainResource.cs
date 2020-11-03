using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QRGainResource", menuName = "DataObjects/Quests/Rewards/QRGainResource", order = 2)]
public class QRGainResource : QuestReward
{
    public int Gold;
    public int Rumors;
    public int Connections;
    public int Progression;
    public int Reputation;

    public override void Claim(Character byCharacter)
    {
        base.Claim(byCharacter);

        byCharacter.TopEmployer.CGold += Gold;
        byCharacter.TopEmployer.CRumors += Rumors;
        byCharacter.TopEmployer.CConnections += Connections;
        byCharacter.TopEmployer.CProgress += Progression;
        byCharacter.TopEmployer.Reputation += Reputation;

        if (byCharacter.TopEmployer == CORE.PC && Reputation != 0)
        {
            TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance("Quest Reward - Reputation "+Reputation,
            ResourcesLoader.Instance.GetSprite("pointing"),
            CORE.PC));
        }

        Transform resourceSource = byCharacter.TopEmployer.CurrentLocation.transform;
        if (Progression > 0)
        {
            CORE.Instance.SplineAnimationObject(
             "ScrollCollectedWorld",
             resourceSource,
             StatsViewUI.Instance.ProgressText.transform,
             () => { StatsViewUI.Instance.RefreshProgress(); },
             false);

            AudioControl.Instance.PlayInPosition("resource_progression", resourceSource.position);
        }

        if (Gold > 0)
        {

            CORE.Instance.SplineAnimationObject(
             "CoinCollectedWorld",
             resourceSource,
             StatsViewUI.Instance.RumorsText.transform,
             () => { StatsViewUI.Instance.RefreshGold(); },
             false);

            AudioControl.Instance.PlayInPosition("resource_gold", resourceSource.position);
        }

        if (Rumors > 0)
        {
            CORE.Instance.SplineAnimationObject(
             "EarCollectedWorld",
             resourceSource,
             StatsViewUI.Instance.RumorsText.transform,
             () => { StatsViewUI.Instance.RefreshRumors(); },
             false);

            AudioControl.Instance.PlayInPosition("resource_rumors", resourceSource.position);
        }

        if (Connections > 0)
        {
            CORE.Instance.SplineAnimationObject(
             "ConnectionCollectedWorld",
             resourceSource,
             StatsViewUI.Instance.ConnectionsText.transform,
             () => { StatsViewUI.Instance.RefreshConnections(); },
             false);

            AudioControl.Instance.PlayInPosition("resource_connections", resourceSource.position);
        }

        if (byCharacter.TopEmployer == CORE.PC)
        {
            if (this.Reputation > 0)
            {
                CORE.Instance.SplineAnimationObject("GoodReputationCollectedWorld",
                  byCharacter.CurrentLocation.transform,
                  StatsViewUI.Instance.transform,
                  null,
                  false);
            }
            else if (this.Reputation < 0)
            {
                CORE.Instance.SplineAnimationObject("BadReputationCollectedWorld",
                  byCharacter.CurrentLocation.transform,
                  StatsViewUI.Instance.transform,
                  null,
                  false);
            }
        }

    }
}