using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOAgentDoingTask", menuName = "DataObjects/Quests/QuestObjectives/QOAgentDoingTask", order = 2)]
public class QOAgentDoingTask : QuestObjective
{
    [SerializeField]
    LongTermTask Task;

    bool valid = false;
    bool subscribed = false;

    public override bool Validate()
    {
        if (!subscribed)
        {
            CORE.Instance.SubscribeToEvent("AgentRefreshedAction", OnAgentAction);
        }

        if(valid)
        {
            subscribed = false;
            CORE.Instance.UnsubscribeFromEvent("AgentRefreshedAction", OnAgentAction);
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnAgentAction()
    {
        List<Character> agents = CORE.Instance.Characters.FindAll((Character charInQuestion) => { return charInQuestion.TopEmployer == CORE.PC; });

        foreach(Character agent in agents)
        {
            if(agent.CurrentTaskEntity != null)
            {
                if(agent.CurrentTaskEntity.CurrentTask == Task)
                {
                    valid = true;
                }
            }
        }
    }
}