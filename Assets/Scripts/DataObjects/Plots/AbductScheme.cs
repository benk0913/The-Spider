using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbductScheme", menuName = "DataObjects/Plots/AbductScheme", order = 2)]
public class AbductScheme : SchemeType
{
    public override void Execute(PlotData data)
    {
        base.Execute(data);
    }

    public override void WinResult(DuelResultData data)
    {
        PopupWindowUI.Instance.AddPopup(new PopupData(GetScenarioPopup(data.Plot.Entry, data.Plot.Method, ExitScenarios), data.Plot.Participants, data.Plot.TargetParticipants,
    () =>
    {
        base.WinResult(data);

        List<LocationEntity> locations = data.Plot.Plotter.TopEmployer.PropertiesOwned;
        LocationEntity location = locations.Find(x => x.HasFreePrisonCell);
        CORE.Instance.Database.GetAgentAction("Get Abducted").Execute(CORE.Instance.Database.GOD, ((PortraitUI)data.Plot.Target).CurrentCharacter, location);
        
    }));
    }
}
