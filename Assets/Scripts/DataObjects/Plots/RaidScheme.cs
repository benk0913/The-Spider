using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RaidScheme", menuName = "DataObjects/Plots/RaidScheme", order = 2)]
public class RaidScheme : SchemeType
{
    public override FailReason Execute(
     Character requester,
     Character plotter,
     List<Character> participants,
     List<Character> targetParticipants,
     AgentInteractable target,
     PlotMethod method,
     PlotEntry entry)
    {

        List<Character> entryTargets = new List<Character>();
        entryTargets.AddRange(targetParticipants);

        List<Character> entryParticipants = new List<Character>();
        entryParticipants.AddRange(participants);

        base.Init(requester, plotter, participants, targetParticipants, target, method, entry);

        //Dueling
        FailReason failureReason = base.Dueling(plotter, participants, entryParticipants, entryTargets, targetParticipants, target, method, entry);

        if (failureReason != null)
        {
            return failureReason;
        }

        return base.Execute(requester, plotter, participants, targetParticipants, target, method, entry);
    }

    public override void WinResult(
        Character requester,
        Character plotter,
        List<Character> participants,
        List<Character> targetParticipants,
        AgentInteractable target,
        PlotMethod method,
        PlotEntry entry)
    {
        LocationEntity targetLocation = (LocationEntity)target;

        PopupWindowUI.Instance.AddPopup(new PopupData(GetScenarioPopup(entry, method, ExitScenarios), participants, targetParticipants,
            () => 
            {
                base.WinResult(requester, plotter, participants, targetParticipants, target, method, entry);

                foreach (Character character in participants)
                {
                    if(targetLocation.Inventory.Count > 0)
                    {
                        character.TopEmployer.Belogings.Add(targetLocation.Inventory[0].Clone());
                        targetLocation.Inventory.RemoveAt(0);
                    }
                }
            }));

    }
}
