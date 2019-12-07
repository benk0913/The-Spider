﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LiberateScheme", menuName = "DataObjects/Plots/LiberateScheme", order = 2)]
public class LiberateScheme : SchemeType
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
        base.WinResult(requester, plotter, participants, targetParticipants, target, method, entry);

        LocationEntity targetLocation = (LocationEntity)target;

        PopupWindowUI.Instance.AddPopup(new PopupData(GetScenarioPopup(entry, method, ExitScenarios), participants, targetParticipants,
            () => 
            {
                foreach (Character prisoner in targetLocation.PrisonersCharacters)
                {
                    prisoner.StopDoingCurrentTask(true);
                    CORE.Instance.Database.GetEventAction("Escape From Prison").Execute(CORE.Instance.Database.GOD, prisoner, prisoner.HomeLocation);
                }
            }));

    }
}
