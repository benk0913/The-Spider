using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LongTermTask", menuName = "DataObjects/AgentActions/LongTermTasks/LongTermTask", order = 2)]
public class LongTermTask : ScriptableObject
{
    [TextArea(2, 3)]
    public string Description;

    public Sprite Icon;

    public int TurnsToComplete = 5;

    public AgentAction[] PossibleResults;
    public AgentAction[] DefaultResults;

    public AgentAction GetResult(Character character)
    {
        List<AgentAction> results = new List<AgentAction>();

        foreach(AgentAction possibleResult in PossibleResults)
        {
            if(possibleResult.RollSucceed(character))
            {
                results.Add(possibleResult);
            }
        }

        if(results.Count > 0)
        {
            return results[Random.Range(0, results.Count)];
        }

        return DefaultResults[Random.Range(0, DefaultResults.Length)];
    }
}
