using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestObjective", menuName = "DataObjects/Quests/QuestObjective", order = 2)]
public class QuestObjective : ScriptableObject
{
    public Sprite Icon;

    public bool IsComplete = false;

    public QuestObjective CreateClone()
    {
        QuestObjective objective = Instantiate(this);
        objective.name = this.name;
        objective.Initialize();

        return objective;
    }

    public void Initialize()
    {

    }

    public void Complete()
    {
        IsComplete = true;
    }
}