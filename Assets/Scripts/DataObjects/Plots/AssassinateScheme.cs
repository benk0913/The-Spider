using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AssassinateScheme", menuName = "DataObjects/Plots/AssassinateScheme", order = 2)]
public class AssassinateScheme : SchemeType
{
    public override void Execute(PlotData data)
    {
        data.TargetParticipants.Add(((PortraitUI)data.Target).CurrentCharacter);
        base.Execute(data);
    }

    public override void WinResult(DuelResultData data)
    {
        PopupWindowUI.Instance.AddPopup(new PopupData(GetScenarioPopup(data.Plot.Entry, data.Plot.Method, ExitScenarios), data.Plot.Participants, data.Plot.TargetParticipants,
    () =>
    {
        base.WinResult(data);

    CORE.Instance.Database.GetEventAction("Death").Execute(CORE.Instance.Database.GOD, ((PortraitUI)data.Plot.Target).CurrentCharacter, ((PortraitUI)data.Plot.Target).CurrentCharacter.CurrentLocation);
    }));
    }
}
