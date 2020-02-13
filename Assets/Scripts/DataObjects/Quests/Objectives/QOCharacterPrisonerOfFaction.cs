using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOCharacterPrisonerOfFaction", menuName = "DataObjects/Quests/QuestObjectives/QOCharacterPrisonerOfFaction", order = 2)]
public class QOCharacterPrisonerOfFaction : QuestObjective
{
    [SerializeField]
    string CharacterID;

    [SerializeField]
    string CharacterName;

    public Faction OfFaction;

    bool valid = false;

    Character CurrentCharacter;
    Faction CurrentFaction;

    public override bool Validate()
    {
        if (CurrentCharacter == null || CurrentFaction == null)
        {
            CurrentCharacter = CORE.Instance.Characters.Find(x => x.ID == CharacterID);

            if (CurrentCharacter == null)
            {
                CurrentCharacter = CORE.Instance.Characters.Find(x => x.name == CharacterName);
            }

            CurrentFaction = CORE.Instance.Factions.Find(x => x.name == OfFaction.name);
        }
        else
        {
            if(CurrentCharacter.PrisonLocation != null 
                && CurrentCharacter.PrisonLocation.OwnerCharacter != null 
                && CurrentCharacter.PrisonLocation.OwnerCharacter.CurrentFaction == CurrentFaction)
            {
                return true;
            }
        }
     
        return false;
    }
}