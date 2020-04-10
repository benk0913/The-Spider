using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

[CreateAssetMenu(fileName = "SessionRule", menuName = "DataObjects/SessionRule", order = 2)]
public class SessionRule : ScriptableObject, ISaveFileCompatible
{
    public PopupDataPreset PopupPreset;
    public int TurnInterval;
    public bool isRepeating = true;
    public int CurrentTurn;
    public DialogDecisionAction Action;

    public void Execute()
    {
        if (PopupPreset != null)
        {
            PopupData popup = new PopupData(PopupPreset, null, null, () => { Action?.Activate(); });
            PopupWindowUI.Instance.AddPopup(popup);
        }
        else
        {
            CORE.Instance.DelayedInvokation(1f,()=>Action?.Activate());
        }
    }

    public void PassTurn(out bool shouldRemove)
    {
        shouldRemove = false;
        CurrentTurn++;

        if(CurrentTurn >= TurnInterval)
        {
            CurrentTurn = 0;
            Execute();

            if(!isRepeating)
            {
                shouldRemove = true;
            }
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
