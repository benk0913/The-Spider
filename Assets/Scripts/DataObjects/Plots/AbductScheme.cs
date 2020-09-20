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

        if (location == null)
        {
            WarningWindowUI.Instance.Show(data.Plot.Plotter.CurrentFaction.name + " faction has no place to hide " + data.Plot.TargetCharacter.name + " so they had decided to ditch the plan at the last moment.",null);
        }
        else
        {
            CORE.Instance.Database.GetAgentAction("Get Abducted").Execute(CORE.Instance.Database.GOD, data.Plot.TargetCharacter, location);
        }

        if (data.Plot.Plotter.TopEmployer == CORE.PC && data.Plot.Corpses.Count > 0)
        {
            CorpseDisposalUI.Instance.Show(data.Plot.Corpses);
        }
    }));
    }
}
