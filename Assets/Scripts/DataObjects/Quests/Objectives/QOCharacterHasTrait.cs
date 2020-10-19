using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOCharacterHasTrait", menuName = "DataObjects/Quests/QuestObjectives/QOCharacterHasTrait", order = 2)]
public class QOCharacterHasTrait : QuestObjective
{
    public Trait TargetTrait;

    public int Amount = 1;

    public bool IsAgent = false;

    bool valid = false;
    bool initialized = false;

    public override bool Validate()
    {
        if(ParentQuest.ForCharacter == null)
        {
            return false;
        }



        if (!initialized)
        {

            CORE.Instance.SubscribeToEvent("PassTimeComplete", OnEventInvoked);

            ValidRefresh();
        }

        if (valid)
        {
            initialized = false;
            CORE.Instance.UnsubscribeFromEvent("PassTimeComplete", OnEventInvoked);
            return true;
        }

        return false;
    }

    void ValidRefresh()
    {
        List<Character> foundCharacters = new List<Character>();

        foundCharacters = CORE.Instance.Characters.FindAll(x => x.Traits.Contains(TargetTrait) && (!IsAgent || (IsAgent && x.TopEmployer == CORE.PC && x.IsAgent)));

        if(foundCharacters.Count < Amount)
        {
            return;
        }
        valid = true;
    }

    void OnEventInvoked()
    {
        ValidRefresh();
    }
    
}