using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestObjective", menuName = "DataObjects/Quests/QuestObjective", order = 2)]
public class QuestObjective : ScriptableObject
{
    public Sprite Icon;

    public bool IsComplete = false;

    [System.NonSerialized]
    public Coroutine ValidateRoutine;

    public string WorldMarkerTarget;

    [System.NonSerialized]
    public GameObject WorldMarker;

    [System.NonSerialized]
    public Quest ParentQuest;

    public bool RandomTarget;

    [System.NonSerialized]
    public LocationEntity TargetLocation;

    [System.NonSerialized]
    public Character TargetCharacter;


    public virtual QuestObjective CreateClone()
    {

        QuestObjective objective = Instantiate(this);
        objective.name = this.name;

        return objective;
    }

    public virtual bool Validate()
    {
        return true;
    }

    public virtual void Complete()
    {
        IsComplete = true;
        QuestsPanelUI.Instance.ObjectiveComplete(this);
    }
}