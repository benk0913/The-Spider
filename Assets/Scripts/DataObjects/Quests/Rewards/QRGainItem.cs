using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QRGainItem", menuName = "DataObjects/Quests/Rewards/QRGainItem", order = 2)]
public class QRGainItem : QuestReward
{
    public Item item;

    public override void Claim(Character byCharacter)
    {
        base.Claim(byCharacter);

        byCharacter.Belogings.Add(item.Clone());
    }
}