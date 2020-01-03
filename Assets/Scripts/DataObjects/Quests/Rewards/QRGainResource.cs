using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QRGainResource", menuName = "DataObjects/Quests/Rewards/QRGainResource", order = 2)]
public class QRGainResource : QuestReward
{
    public int Gold;
    public int Rumors;
    public int Connections;
    public int Progression;

    public override void Claim(Character byCharacter)
    {
        base.Claim(byCharacter);

        byCharacter.TopEmployer.Gold += Gold;
        byCharacter.TopEmployer.Rumors += Rumors;
        byCharacter.TopEmployer.Connections += Connections;
        byCharacter.TopEmployer.Progress += Progression;
    }
}