using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QRRelationWithFaction", menuName = "DataObjects/Quests/Rewards/QRRelationWithFaction", order = 2)]
public class QRRelationWithFaction : QuestReward
{
    public Faction WithFaction;
    public int Value;

    public override void Claim(Character byCharacter)
    {
        base.Claim(byCharacter);

        Faction faction = CORE.Instance.Factions.Find(x => x.name == WithFaction.name);

        if(faction == null)
        {
            Debug.LogError("Couldn't Find Faction "+ WithFaction.name);
            return;
        }
        
        
        faction.Relations.GetRelations(byCharacter.CurrentFaction).TotalValue += Value;
    }
}