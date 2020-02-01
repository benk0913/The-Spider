using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

[CreateAssetMenu(fileName = "SessionRule", menuName = "DataObjects/SessionRule", order = 2)]
public class SessionRule : ScriptableObject, ISaveFileCompatible
{
    public PopupDataPreset PopupPreset;
    public int TurnInterval;
    public int CurrentTurn;
    public DialogDecisionAction Action;

    public void Execute()
    {
        PopupData popup = new PopupData(PopupPreset, null, null, () => { Action.Activate(); });
        PopupWindowUI.Instance.AddPopup(popup);
    }

    public void PassTurn()
    {
        CurrentTurn++;

        if(CurrentTurn >= TurnInterval)
        {
            CurrentTurn = 0;
            Execute();
        }
    }

    public SessionRule Clone()
    {
        SessionRule tempRule = Instantiate(this);
        tempRule.name = this.name;
        tempRule.CurrentTurn = 0;

        return tempRule;
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        node["Name"] = this.name;
        node["CurrentTurn"] = CurrentTurn.ToString();

        return node;
    }

    public void FromJSON(JSONNode node)
    {
        CurrentTurn = int.Parse(node["CurrentTurn"]);
    }

    public void ImplementIDs()
    {
        
    }
}
