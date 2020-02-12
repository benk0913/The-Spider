using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOFactionHeadIsPuppet", menuName = "DataObjects/Quests/QuestObjectives/QOFactionHeadIsPuppet", order = 2)]
public class QOFactionHeadIsPuppet : QuestObjective
{
    [SerializeField]
    Faction PuppetFaction;

    [SerializeField]
    Faction PuppetOf;

    bool valid = false;

    Character CurrentCharacter;

    Faction CurrentFaction;

    public override bool Validate()
    {
        if (CurrentCharacter == null && CurrentFaction == null)
        {
            CurrentCharacter = CORE.Instance.Characters.Find(x => x.name == PuppetFaction.FactionHead.name);

            CurrentFaction = CORE.Instance.Factions.Find(x => x.name == PuppetOf.name);
        }
        else
        {
            if(CurrentCharacter.IsDead || CurrentCharacter.CurrentFaction.name != PuppetFaction.name)
            {
                CurrentCharacter = null;
                return false;
            }

            if(CurrentCharacter.PuppetOf == CurrentFaction)
            {
                return true;
            }
        }
     
        return false;
    }
}