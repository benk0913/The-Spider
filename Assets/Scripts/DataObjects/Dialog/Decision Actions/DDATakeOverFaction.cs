using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDATakeOverFaction", menuName = "DataObjects/Dialog/Actions/DDATakeOverFaction", order = 2)]
public class DDATakeOverFaction : DialogDecisionAction
{
    [SerializeField]
    Faction faction;

    public override void Activate()
    {
        Character factionHead = CORE.Instance.Characters.Find(x => x.CurrentFaction.name == faction.name && x.name == faction.FactionHead.name);

        if(factionHead == null)
        {
            Debug.LogError("Couldn't find faction head for faction: " + faction.name);
            return;
        }

        factionHead.StartWorkingFor(CORE.PC.PropertiesOwned[0]);
        factionHead.AI = null;
    }
}
