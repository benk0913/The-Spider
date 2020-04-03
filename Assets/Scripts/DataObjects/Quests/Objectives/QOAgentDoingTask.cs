using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOAgentDoingTask", menuName = "DataObjects/Quests/QuestObjectives/QOAgentDoingTask", order = 2)]
public class QOAgentDoingTask : QuestObjective
{
    [SerializeField]
    LongTermTask Task;

    [SerializeField]
    Character TargetCharacter;


    bool valid = false;
    bool subscribed = false;

    public override bool Validate()
    {
        if (!subscribed)
        {
            CORE.Instance.SubscribeToEvent("PassTimeStarted", OnAgentAction);
            subscribed = true;
        }

        if(valid)
        {
            subscribed = false;
            CORE.Instance.UnsubscribeFromEvent("PassTimeStarted", OnAgentAction);
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnAgentAction()
    {
        if(valid)
        {
            return;
        }

        if(ParentQuest.ForCharacter == null)
        {
            return;
        }

        List<Character> agents = ParentQuest.ForCharacter.CharactersInCommand;

        foreach (Character agent in agents)
        {
            if(agent.CurrentTaskEntity != null)
            {
                if(agent.CurrentTaskEntity.CurrentTask == Task)
                {
                    if(TargetCharacter!=null && agent.CurrentTaskEntity.TargetCharacter.name != TargetCharacter.name)
                    {
                        continue;
                    }

                    valid = true;
                    break;
                }
            }
        }
    }
}