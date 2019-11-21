using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestReward", menuName = "DataObjects/Quests/QuestReward", order = 2)]
public class QuestReward : ScriptableObject
{
    public Sprite Icon;

    public virtual QuestReward CreateClone()
    {
        QuestReward reward = Instantiate(this);
        reward.name = this.name;

        return reward;
    }

    public virtual void Claim(Character byCharacter)
    {
        
    }
}