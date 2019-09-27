using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DialogDecision", menuName = "DataObjects/Dialog/DialogDecision", order = 2)]
public class DialogDecision : ScriptableObject
{
    public string Title;

    [Tooltip("Conditions - should the decision appear?")]
    public List<DialogDecisionCondition> AppearanceConditions = new List<DialogDecisionCondition>();

    [Tooltip("Conditions - is the decision available?")]
    public List<DialogDecisionCondition> ActiveConditions = new List<DialogDecisionCondition>();

    [Tooltip("Actions - which should always execute for decision.")]
    public List<DialogDecisionAction> Actions = new List<DialogDecisionAction>();

}
