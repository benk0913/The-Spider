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

        byCharacter.TopEmployer.Gold += Gold;
        byCharacter.TopEmployer.Rumors += Rumors;
        byCharacter.TopEmployer.Connections += Connections;
        byCharacter.TopEmployer.Progress += Progression;
        byCharacter.TopEmployer.Reputation += Reputation;

        Transform resourceSource = byCharacter.TopEmployer.CurrentLocation.transform;
        if (Progression > 0)
        {
            CORE.Instance.SplineAnimationObject(
             "ScrollCollectedWorld",
             resourceSource,
             StatsViewUI.Instance.ProgressText.transform,
             () => { StatsViewUI.Instance.RefreshProgress(); },
             false);
        }

        if (Gold > 0)
        {

            CORE.Instance.SplineAnimationObject(
             "CoinCollectedWorld",
             resourceSource,
             StatsViewUI.Instance.RumorsText.transform,
             () => { StatsViewUI.Instance.RefreshGold(); },
             false);
        }

        if (Rumors > 0)
        {
            CORE.Instance.SplineAnimationObject(
             "EarCollectedWorld",
             resourceSource,
             StatsViewUI.Instance.RumorsText.transform,
             () => { StatsViewUI.Instance.RefreshRumors(); },
             false);
        }

        if (Connections > 0)
        {
            CORE.Instance.SplineAnimationObject(
             "ConnectionCollectedWorld",
             resourceSource,
             StatsViewUI.Instance.ConnectionsText.transform,
             () => { StatsViewUI.Instance.RefreshConnections(); },
             false);
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