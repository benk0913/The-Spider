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
            tempGold = Mathf.RoundToInt(CORE.PC.CGold * (Gold / 100f));
            tempRumors = Mathf.RoundToInt(CORE.PC.CRumors * (Rumors / 100f));
            tempConnections = Mathf.RoundToInt(CORE.PC.CConnections * (Connections / 100f));
            tempProgression = Mathf.RoundToInt(CORE.PC.CProgress * (Progression / 100f));
            tempReputation = Mathf.RoundToInt(CORE.PC.Reputation * (Reputation / 100f));
        }

        if(tempGold > 0)
        {
            GlobalMessagePrompterUI.Instance.Show("Gained " + tempGold + " Gold", 1f, Color.yellow);
        }
        else if (tempGold < 0)
        {
            GlobalMessagePrompterUI.Instance.Show("Lost " + tempGold + " Gold", 1f, Color.red);
        }

        if (tempGold > 0)
        {
            GlobalMessagePrompterUI.Instance.Show("Gained " + tempGold + " Gold", 1f, Color.yellow);
        }
        else if (tempGold < 0)
        {
            GlobalMessagePrompterUI.Instance.Show("Lost " + tempGold + " Gold", 1f, Color.red);
        }

        if (tempRumors > 0)
        {
            GlobalMessagePrompterUI.Instance.Show("Gained " + tempRumors + " Rumors", 1f, Color.yellow);
        }
        else if (tempRumors < 0)
        {
            GlobalMessagePrompterUI.Instance.Show("Lost " + tempRumors + " Rumors", 1f, Color.red);
        }

        if (tempConnections > 0)
        {
            GlobalMessagePrompterUI.Instance.Show("Gained " + tempConnections + " Connection", 1f, Color.yellow);
        }
        else if (tempConnections < 0)
        {
            GlobalMessagePrompterUI.Instance.Show("Lost " + tempConnections + " Connection", 1f, Color.red);
        }

        if (tempProgression > 0)
        {
            GlobalMessagePrompterUI.Instance.Show("Gained " + tempProgression + " Progression", 1f, Color.yellow);
        }
        else if (tempProgression < 0)
        {
            GlobalMessagePrompterUI.Instance.Show("Lost " + tempProgression + " Progression", 1f, Color.red);
        }

        if (tempReputation > 0)
        {
            GlobalMessagePrompterUI.Instance.Show("Gained " + tempReputation + " Reputation", 1f, Color.yellow);
        }
        else if (tempReputation < 0)
        {
            GlobalMessagePrompterUI.Instance.Show("Lost " + tempReputation + " Reputation", 1f, Color.red);
        }

        CORE.PC.TopEmployer.CGold += tempGold;
        CORE.PC.TopEmployer.CRumors += tempRumors;
        CORE.PC.TopEmployer.CConnections += tempConnections;
        CORE.PC.TopEmployer.CProgress += tempProgression;
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

            AudioControl.Instance.PlayInPosition("resource_progression", resourceSource.position);
        }

        if (tempGold > 0)
        {

            CORE.Instance.SplineAnimationObject(
             "CoinCollectedWorld",
             resourceSource,
             StatsViewUI.Instance.GoldText.transform,
             () => { StatsViewUI.Instance.RefreshGold(); },
             false);

            AudioControl.Instance.PlayInPosition("resource_gold", resourceSource.position);
        }

        if (tempRumors > 0)
        {
            CORE.Instance.SplineAnimationObject(
             "EarCollectedWorld",
             resourceSource,
             StatsViewUI.Instance.RumorsText.transform,
             () => { StatsViewUI.Instance.RefreshRumors(); },
             false);

            AudioControl.Instance.PlayInPosition("resource_rumors", resourceSource.position);
        }

        if (tempConnections > 0)
        {
            CORE.Instance.SplineAnimationObject(
             "ConnectionCollectedWorld",
             resourceSource,
             StatsViewUI.Instance.ConnectionsText.transform,
             () => { StatsViewUI.Instance.RefreshConnections(); },
             false);

            AudioControl.Instance.PlayInPosition("resource_connections", resourceSource.position);
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
