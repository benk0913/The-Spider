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

        int gold = 0;
        int connections = 0;
        int rumors = 0;
        int prog = 0;

        if (targetLocation.OwnerCharacter != null && targetLocation.OwnerCharacter.TopEmployer != null)
        {
            gold = Mathf.FloorToInt(targetLocation.OwnerCharacter.TopEmployer.CGold * 0.2f);
            connections = Mathf.FloorToInt(targetLocation.OwnerCharacter.TopEmployer.CConnections * 0.2f);
            rumors = Mathf.FloorToInt(targetLocation.OwnerCharacter.TopEmployer.CRumors * 0.2f);
            prog = Mathf.FloorToInt(targetLocation.OwnerCharacter.TopEmployer.CProgress * 0.2f);

            targetLocation.OwnerCharacter.TopEmployer.CGold -= gold;
            targetLocation.OwnerCharacter.TopEmployer.CConnections -= connections;
            targetLocation.OwnerCharacter.TopEmployer.CRumors -= rumors;
            targetLocation.OwnerCharacter.TopEmployer.CProgress -= prog;

            data.Plot.Plotter.TopEmployer.CGold += gold;
            data.Plot.Plotter.TopEmployer.CRumors += rumors;
            data.Plot.Plotter.TopEmployer.CConnections += connections;
            data.Plot.Plotter.TopEmployer.CProgress += prog;
        }

        if (targetLocation.OwnerCharacter != null && targetLocation.OwnerCharacter.TopEmployer == CORE.PC)
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

            WarningWindowUI.Instance.Show("20% of your resources were looted by the plotters.", null);
        }
        else if(data.Plot.Plotter.TopEmployer == CORE.PC)
        {
            WarningWindowUI.Instance.Show("You managed to steal 20% of their resources!", null);
        }
        }));

    }
}
