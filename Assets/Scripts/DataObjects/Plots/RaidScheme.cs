using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RaidScheme", menuName = "DataObjects/Plots/RaidScheme", order = 2)]
public class RaidScheme : SchemeType
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

                foreach (Character character in data.Plot.Participants)
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
