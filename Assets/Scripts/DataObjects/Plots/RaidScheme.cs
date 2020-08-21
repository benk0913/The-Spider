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
        if (targetLocation.Inventory.Count > 0)
        {
            character.TopEmployer.Belogings.Add(targetLocation.Inventory[0].Clone());
            targetLocation.Inventory.RemoveAt(0);
        }
    }

    if (targetLocation.OwnerCharacter.TopEmployer == CORE.PC)
    {

        List<Item> stoledItems = new List<Item>();
        foreach (Character character in data.Plot.Participants)
        {
            Item tempItem = CORE.PC.Belogings.Find(x => x.Sellable);

            if (tempItem == null)
            {
                break;
            }

            stoledItems.Add(tempItem);

            if (character.TopEmployer.PropertiesOwned[0] != null)
            {
                character.TopEmployer.PropertiesOwned[0].Inventory.Add(tempItem);
            }
            else
            {
                character.TopEmployer.Belogings.Add(tempItem);
            }
            CORE.PC.Belogings.Remove(tempItem);
        }

        string stoled = "";
        stoledItems.ForEach((x) => stoled += " | " + x.name);

        WarningWindowUI.Instance.Show("The following items have been stolen from your personal inventory:"+stoled,null);
                }
            }));

    }
}
