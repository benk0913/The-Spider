using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LongTermTask", menuName = "DataObjects/AgentActions/LongTermTask", order = 2)]
public class LongTermTask : ScriptableObject
{
    [TextArea(6, 10)]
    public string Description;

    public Sprite Icon;

    public int TurnsToComplete = 5;

    public AgentAction[] PossibleResults;
    public AgentAction[] DefaultResults;

    public bool Cancelable = true;

    public bool ExecuteAtHome = false;

    public bool HideFromLog = false;

    public Trait[] TraitsToTargetDuringAction;

    public AgentAction GetResult(Character character)
    {
        List<AgentAction> results = new List<AgentAction>();
        AgentAction finalResult;

        foreach (AgentAction possibleResult in PossibleResults)
        {
            if(possibleResult.RollSucceed(character))
            {
                results.Add(possibleResult);
            }
        }

        if(results.Count > 0)
        {
            finalResult = results[Random.Range(0, results.Count)];

            if (!HideFromLog && character.TopEmployer == CORE.PC)
            {
                TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance(this.name + ": <color=yellow>" + finalResult.name + "</color>", Icon, character));
            }

            return finalResult;
        }

        finalResult = DefaultResults[Random.Range(0, DefaultResults.Length)];

        if (!HideFromLog && character.TopEmployer == CORE.PC)
        {
            TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance(this.name + ": <color=yellow>" + finalResult.name + "</color>", Icon, character));
        }
        return finalResult;
    }
}
