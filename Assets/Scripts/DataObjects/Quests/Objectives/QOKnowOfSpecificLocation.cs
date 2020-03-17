using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOKnowOfSpecificLocation", menuName = "DataObjects/Quests/QuestObjectives/QOKnowOfSpecificLocation", order = 2)]
public class QOKnowOfSpecificLocation : QuestObjective
{
    bool valid = false;
    bool subscribed = false;

    public Property OfProperty;

    public override bool Validate()
    {
        if(CORE.Instance.Locations.Find(x => x.CurrentProperty == OfProperty && x.Known.IsKnown("Existance",CORE.PC)) != null)
        {
            return true;
        }

        return false;
    }

    public override GameObject GetMarkerTarget()
    {
        return CORE.Instance.Locations.Find(x => x.CurrentProperty.name == OfProperty.name).gameObject;
    }

}