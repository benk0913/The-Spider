using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAGainResources", menuName = "DataObjects/Dialog/Actions/DDAGainResources", order = 2)]
public class DDAGainResources : DialogDecisionAction
{
    public int Gold;
    public int Rumors;
    public int Connections;
    public int Progression;
    public int Reputation;
    public bool inPrecent = false;

    public override void Activate()
    {
        int tempGold = Gold;
        int tempRumors = Rumors;
        int tempConnections = Connections;
        int tempProgression = Progression;
        int tempReputation = Reputation;

        if (inPrecent)
        {
            tempGold = Mathf.RoundToInt(CORE.PC.Gold * (Gold / 100f));
            tempRumors = Mathf.RoundToInt(CORE.PC.Rumors * (Rumors / 100f));
            tempConnections = Mathf.RoundToInt(CORE.PC.Connections * (Connections / 100f));
            tempProgression = Mathf.RoundToInt(CORE.PC.Progress * (Progression / 100f));
            tempReputation = Mathf.RoundToInt(CORE.PC.Reputation * (Reputation / 100f));
        }


        CORE.PC.TopEmployer.Gold += tempGold;
        CORE.PC.TopEmployer.Rumors += tempRumors;
        CORE.PC.TopEmployer.Connections += tempConnections;
        CORE.PC.TopEmployer.Progress += tempProgression;
        CORE.PC.TopEmployer.Reputation += tempReputation;

        Transform resourceSource = CORE.PC.TopEmployer.CurrentLocation.transform;
        if (tempProgression > 0)
        {
            CORE.Instance.SplineAnimationObject(
             "ScrollCollectedWorld",
             resourceSource,
             StatsViewUI.Instance.ProgressText.transform,
             () => { StatsViewUI.Instance.RefreshProgress(); },
             false);
        }

        if (tempGold > 0)
        {

            CORE.Instance.SplineAnimationObject(
             "CoinCollectedWorld",
             resourceSource,
             StatsViewUI.Instance.RumorsText.transform,
             () => { StatsViewUI.Instance.RefreshGold(); },
             false);
        }

        if (tempRumors > 0)
        {
            CORE.Instance.SplineAnimationObject(
             "EarCollectedWorld",
             resourceSource,
             StatsViewUI.Instance.RumorsText.transform,
             () => { StatsViewUI.Instance.RefreshRumors(); },
             false);
        }

        if (tempConnections > 0)
        {
            CORE.Instance.SplineAnimationObject(
             "ConnectionCollectedWorld",
             resourceSource,
             StatsViewUI.Instance.ConnectionsText.transform,
             () => { StatsViewUI.Instance.RefreshConnections(); },
             false);
        }

        if (CORE.PC.TopEmployer == CORE.PC)
        {
            if (tempReputation > 0)
            {
                CORE.Instance.SplineAnimationObject("GoodReputationCollectedWorld",
                  CORE.PC.CurrentLocation.transform,
                  StatsViewUI.Instance.transform,
                  null,
                  false);
            }
            else if (tempReputation < 0)
            {
                CORE.Instance.SplineAnimationObject("BadReputationCollectedWorld",
                  CORE.PC.CurrentLocation.transform,
                  StatsViewUI.Instance.transform,
                  null,
                  false);
            }
        }

    }
}
