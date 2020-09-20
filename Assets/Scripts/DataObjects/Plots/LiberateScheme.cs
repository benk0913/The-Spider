using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LiberateScheme", menuName = "DataObjects/Plots/LiberateScheme", order = 2)]
public class LiberateScheme : SchemeType
{
    public override void Execute(PlotData data)
    {
        base.Execute(data);
    }

    public override void WinResult(DuelResultData data)
    {
        LocationEntity targetLocation = (LocationEntity)data.Plot.Target;

        PopupWindowUI.Instance.AddPopup(new PopupData(GetScenarioPopup(data.Plot.Entry, data.Plot.Method, ExitScenarios), data.Plot.Participants, data.Plot.TargetParticipants,
   () =>
   {
                base.WinResult(data);

                foreach (Character prisoner in targetLocation.PrisonersCharacters)
                {
                    CORE.Instance.DelayedInvokation(0.1f, () =>
                    {
                        prisoner.StopDoingCurrentTask(true);
                        CORE.Instance.Database.GetAgentAction("Get Liberated").Execute(CORE.Instance.Database.GOD, prisoner, prisoner.HomeLocation);
                    });
                }

       if (data.Plot.Plotter.TopEmployer == CORE.PC && data.Plot.Corpses.Count > 0)
       {
           CorpseDisposalUI.Instance.Show(data.Plot.Corpses);
       }
   }));

    }
}
