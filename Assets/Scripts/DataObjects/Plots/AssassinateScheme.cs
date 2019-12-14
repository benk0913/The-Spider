using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AssassinateScheme", menuName = "DataObjects/Plots/AssassinateScheme", order = 2)]
public class AssassinateScheme : SchemeType
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
        FailReason failureReason = base.Dueling(plotter,participants,entryParticipants,entryTargets,targetParticipants,target,method,entry);

        if (failureReason != null)
        {
            return failureReason;
        }

        return base.Execute(requester,plotter,participants,targetParticipants,target,method,entry);
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
        PopupWindowUI.Instance.AddPopup(new PopupData(GetScenarioPopup(entry, method, ExitScenarios), participants, targetParticipants,
    () =>
    {
        base.WinResult(requester, plotter, participants, targetParticipants, target, method, entry);

        CORE.Instance.Database.GetEventAction("Death").Execute(CORE.Instance.Database.GOD, ((PortraitUI)target).CurrentCharacter, ((PortraitUI)target).CurrentCharacter.CurrentLocation);
    }));
    }
}
