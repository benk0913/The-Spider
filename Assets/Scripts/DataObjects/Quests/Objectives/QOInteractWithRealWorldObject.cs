using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOInteractWithRealWorldObject", menuName = "DataObjects/Quests/QuestObjectives/QOInteractWithRealWorldObject", order = 2)]
public class QOInteractWithRealWorldObject : QuestObjective
{
    public string ObjectName;

    public InteractableEntity Entity;

    bool interacted = false;

    public override bool Validate()
    {
        if(Entity == null)
        {
            GameObject tempObj = GameObject.Find(ObjectName);

            if(tempObj == null)
            {
                return false;
            }

            Entity = tempObj.GetComponent<InteractableEntity>();

            if(Entity == null)
            {
                return false;
            }

            Entity.Actions.AddListener(OnInteract);
        }

        return interacted;
    }

    void OnInteract()
    {
        Entity.Actions.RemoveListener(OnInteract);
        interacted = true;
    }

    public override GameObject GetMarkerTarget()
    {
        return GameObject.Find(ObjectName);
    }
}