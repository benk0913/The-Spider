using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOCharacterIsPuppet", menuName = "DataObjects/Quests/QuestObjectives/QOCharacterIsPuppet", order = 2)]
public class QOCharacterIsPuppet : QuestObjective
{
    [SerializeField]
    string CharacterID;

    [SerializeField]
    string CharacterName;

    [SerializeField]
    Faction PuppetOf;

    bool valid = false;

    Character CurrentCharacter;

    Faction CurrentFaction;

    public override bool Validate()
    {
        if (CurrentCharacter == null && CurrentFaction == null)
        {
            CurrentCharacter = CORE.Instance.Characters.Find(x => x.ID == CharacterID);

            if (CurrentCharacter == null)
            {
                CurrentCharacter = CORE.Instance.Characters.Find(x => x.name == CharacterName);
            }

            CurrentFaction = CORE.Instance.Factions.Find(x => x.name == PuppetOf.name);
        }
        else
        {
            if(CurrentCharacter.PuppetOf == CurrentFaction)
            {
                return true;
            }
        }
     
        return false;
    }
}