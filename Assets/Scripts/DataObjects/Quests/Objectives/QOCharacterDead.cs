using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOCharacterDead", menuName = "DataObjects/Quests/QuestObjectives/QOCharacterDead", order = 2)]
public class QOCharacterDead : QuestObjective
{
    [SerializeField]
    string CharacterID;

    [SerializeField]
    string CharacterName;

    bool valid = false;

    Character CurrentCharacter;

    public override bool Validate()
    {
        if (CurrentCharacter == null)
        {
            CurrentCharacter = CORE.Instance.Characters.Find(x => x.ID == CharacterID);

            if (CurrentCharacter == null)
            {
                CurrentCharacter = CORE.Instance.Characters.Find(x => x.name == CharacterName);
            }
        }
        else
        {
            if(CurrentCharacter.IsDead)
            {
                return true;
            }
        }
     
        return false;
    }
}