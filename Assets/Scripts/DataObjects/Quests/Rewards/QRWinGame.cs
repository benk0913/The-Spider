using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QRWinGame", menuName = "DataObjects/Quests/Rewards/QRWinGame", order = 2)]
public class QRWinGame : QuestReward
{
    public override void Claim(Character byCharacter)
    {
        base.Claim(byCharacter);

        if(byCharacter == CORE.PC)
        {
            VictoryWindowUI.Instance.Show();
        }
        else
        {
            LoseWindowUI.Instance.Show();
        }
    }
}