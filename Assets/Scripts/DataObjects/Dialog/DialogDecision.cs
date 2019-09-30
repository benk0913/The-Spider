using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DialogDecision", menuName = "DataObjects/Dialog/DialogDecision", order = 2)]
public class DialogDecision : ScriptableObject
{
    public string Title
    {
        get
        {
            if (string.IsNullOrEmpty(CustomTitle))
            {
                return this.name;
            }

            return CustomTitle;
        }
    }

    [SerializeField]
    string CustomTitle = "";

    public Sprite Icon;

    [Expandable]
    [Tooltip("Conditions - should the decision appear?")]
    public List<DialogDecisionCondition> AppearanceConditions = new List<DialogDecisionCondition>();

    [Expandable]
    [Tooltip("Conditions - is the decision available?")]
    public List<DialogDecisionCondition> ActiveConditions = new List<DialogDecisionCondition>();

    [Expandable]
    [Tooltip("Actions - which should always execute for decision.")]
    public List<DialogDecisionAction> Actions = new List<DialogDecisionAction>();

    public DialogPiece NextPiece;

    public void Activate()
    {
        foreach(DialogDecisionAction action in Actions)
        {
            action.Activate();
        }

        if (NextPiece != null)
        {
            DialogWindowUI.Instance.ShowDialogPiece(NextPiece);
        }
    }

    public DialogDecision Clone()
    {
        DialogDecision newDecision = Instantiate(this);
        newDecision.name = this.name;

        return newDecision;
    }
}
