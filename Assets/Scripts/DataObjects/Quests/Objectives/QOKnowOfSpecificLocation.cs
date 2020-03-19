using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOKnowOfSpecificLocation", menuName = "DataObjects/Quests/QuestObjectives/QOKnowOfSpecificLocation", order = 2)]
public class QOKnowOfSpecificLocation : QuestObjective
{
    bool valid = false;
    bool subscribed = false;

    public Property OfProperty;

    public bool MarkClosestLocation = false;

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
        if(MarkClosestLocation)
        {
            List<LocationEntity> targets = CORE.Instance.Locations.FindAll(x => x.CurrentProperty.name == OfProperty.name);

            float distance = Mathf.Infinity;
            LocationEntity target = null;
            LocationEntity hostLocation = CORE.PC.CurrentLocation;

            foreach (LocationEntity location in targets)
            {
                float currentDist = Vector3.Distance(hostLocation.gameObject.transform.position, location.gameObject.transform.position);

                if(currentDist < distance)
                {
                    distance = currentDist;
                    target = location;
                }
            }

            if(target == null)
            {
                return null;
            }

            return target.gameObject;
        }

        return CORE.Instance.Locations.Find(x => x.CurrentProperty.name == OfProperty.name).gameObject;
    }

}