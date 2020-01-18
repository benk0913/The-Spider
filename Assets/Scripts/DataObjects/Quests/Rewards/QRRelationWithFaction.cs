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

        CORE.Instance.Factions.Find(x => x.name == WithFaction.name).Relations.GetRelations(byCharacter.CurrentFaction).TotalValue += Value;
    }
}