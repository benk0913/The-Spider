using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOInformationGathered", menuName = "DataObjects/Quests/QuestObjectives/QOInformationGathered", order = 2)]
public class QOInformationGathered : QuestObjective
{
    bool valid = false;
    bool initialized = false;

    public string InformationKey = "CurrentLocation";
    public string CharacterName = "Jeremy Rowanson";

    Character Target;

    public override bool Validate()
    {
        if (!initialized)
        {

            CORE.Instance.SubscribeToEvent("PassTimeComplete", OnEventInvoked);

            Target = CORE.Instance.Characters.Find((Character charInQuestion) => { return charInQuestion.name == CharacterName; });

            if (Target == null)
            {
                Debug.LogError("CAN'T FIND " + CharacterName);
                valid = true;
                return true;
            }
        }

        if(valid)
        {
            initialized = false;
            CORE.Instance.UnsubscribeFromEvent("PassTimeComplete", OnEventInvoked);
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnEventInvoked()
    {
        if (Target.IsKnown(InformationKey, CORE.PC))
        {
            valid = true;
        }
    }
}