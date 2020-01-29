using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PiracyComplete", menuName = "DataObjects/AgentActions/Work/PiracyComplete", order = 2)]
public class PiracyComplete : WorkComplete
{

    public override void EarnGold(Character requester, Character character, AgentInteractable target, int addedGold = 0)
    {
        base.EarnGold(requester, character,target, addedGold);

        if (character.WorkLocation == null)
        {
            return;
        }

        int propertiesNotYours = CORE.Instance.Locations.FindAll(x =>
               (x.CurrentProperty.PlotType.name == "Naval" || x.CurrentProperty.PlotType.name == "Coastal")
            && (x.OwnerCharacter != null && x.OwnerCharacter.TopEmployer != character.TopEmployer)
            ).Count;

        if(propertiesNotYours <= 0)
        {
            return;
        }

        float earnedGold =
            character.WorkLocation.CurrentAction.GoldGenerated * propertiesNotYours
            * CORE.Instance.Database.Stats.GlobalRevenueMultiplier;

        if (Mathf.RoundToInt(earnedGold) > 0)
        {
            if (character.TopEmployer == null)
            {
                character.TopEmployer.Gold += Mathf.RoundToInt(earnedGold + addedGold);
                return;
            }

            if (character.TopEmployer == CORE.PC)
            {
                CORE.Instance.ShowHoverMessage("Bonus: " + (character.WorkLocation.CurrentAction.GoldGenerated * CORE.Instance.Database.Stats.GlobalRevenueMultiplier) + " X " + propertiesNotYours, Icon, character.CurrentLocation.transform);

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
