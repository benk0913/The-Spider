using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GainResources", menuName = "DataObjects/AgentActions/GainResources", order = 2)]
public class GainResources : AgentAction //DO NOT INHERIT FROM
{
    public int Gold;
    public int Connections;
    public int Rumors;
    public int Progression;
    public int Reputation;
    public List<Item> Items = new List<Item>();

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
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

        base.Execute(requester, character, target);
    }

    public override void ExecuteResult(Character requester, Character character, AgentInteractable target)
    {
        base.ExecuteResult(requester, character, target);

        Transform resourceSource;

        if (target.GetType() == typeof(LocationEntity))
        {
            resourceSource = ((LocationEntity)target).transform;
        }
        else
        {
            resourceSource = ((PortraitUI)target).CurrentCharacter.CurrentLocation.transform;
        }

        if (Progression != 0)
        {
            character.TopEmployer.Progress += Progression;

            CORE.Instance.SplineAnimationObject(
             "PaperCollectedWorld",
             resourceSource,
             StatsViewUI.Instance.ProgressText.transform,
             () => { StatsViewUI.Instance.RefreshProgress(); },
             false);
        }

        if (Gold != 0)
        {
            character.TopEmployer.Gold += Gold;

            CORE.Instance.SplineAnimationObject(
             "CoinCollectedWorld",
             resourceSource,
             StatsViewUI.Instance.RumorsText.transform,
             () => { StatsViewUI.Instance.RefreshGold(); },
             false);
        }

        if (Rumors != 0)
        {
            character.TopEmployer.Rumors += Rumors;

            CORE.Instance.SplineAnimationObject(
             "EarCollectedWorld",
             resourceSource,
             StatsViewUI.Instance.RumorsText.transform,
             () => { StatsViewUI.Instance.RefreshRumors(); },
             false);
        }

        if (Connections != 0)
        {
            character.TopEmployer.Connections += Connections;

            CORE.Instance.SplineAnimationObject(
             "ConnectionCollectedWorld",
             resourceSource,
             StatsViewUI.Instance.ConnectionsText.transform,
             () => { StatsViewUI.Instance.RefreshConnections(); },
             false);
        }

        requester.Reputation += this.Reputation;
        Items.ForEach((x) => requester.Belogings.Add(x.Clone()));
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        return true;
    }
}
