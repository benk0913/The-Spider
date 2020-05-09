using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOQuestionedCharacter", menuName = "DataObjects/Quests/QuestObjectives/QOQuestionedCharacter", order = 2)]
public class QOQuestionedCharacter : QuestObjective
{
    public Character TargetCharacter;

    Character CurrentCharacter;

    public override bool Validate()
    {
        if (ParentQuest.ForCharacter == null)
        {
            return false;
        }
    
        if (CurrentCharacter == null)
        {
            CurrentCharacter = CORE.Instance.Characters.Find(x => x.name == TargetCharacter.name);
            return false;
        }

        if(CurrentCharacter.CurrentQuestioningInstance != null)
        {
            return false;
        }

        return true;
    }
    
}