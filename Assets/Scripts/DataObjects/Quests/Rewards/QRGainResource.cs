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

        byCharacter.TopEmployer.Progress += Gold;
        byCharacter.TopEmployer.Progress += Rumors;
        byCharacter.TopEmployer.Progress += Connections;
        byCharacter.TopEmployer.Progress += Progression;
    }
}