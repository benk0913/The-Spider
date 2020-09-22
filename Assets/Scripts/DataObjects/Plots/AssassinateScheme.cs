using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AssassinateScheme", menuName = "DataObjects/Plots/AssassinateScheme", order = 2)]
public class AssassinateScheme : SchemeType
{
    public override void Execute(PlotData data)
    {
        //data.TargetParticipants.Add(((PortraitUI)data.Target).CurrentCharacter);
        base.Execute(data);
    }

    public override void WinResult(DuelResultData data)
    {
        PopupWindowUI.Instance.AddPopup(new PopupData(GetScenarioPopup(data.Plot.Entry, data.Plot.Method, ExitScenarios), data.Plot.Participants, data.Plot.TargetParticipants,
    () =>
    {
        base.WinResult(data);

    CORE.Instance.Database.GetAgentAction("Death").Execute(CORE.Instance.Database.GOD, data.Plot.TargetCharacter, data.Plot.TargetCharacter.CurrentLocation);

        data.Plot.AddCorpse(data.Plot.TargetCharacter);

        if (data.Plot.Plotter.TopEmployer == CORE.PC && data.Plot.Corpses.Count > 0)
        {
            CorpseDisposalUI.Instance.Show(data.Plot.Corpses);
        }

    }));
    }
}
