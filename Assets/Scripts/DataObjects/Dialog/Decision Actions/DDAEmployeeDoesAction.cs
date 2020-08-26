using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAEmployeeDoesAction", menuName = "DataObjects/Dialog/Actions/DDAEmployeeDoesAction", order = 2)]
public class DDAEmployeeDoesAction : DialogDecisionAction
{
    public Property TheProperty;
    public Faction OfFaction;
    public AgentAction ActionToDo;

    public override void Activate()
    {
        LocationEntity targetLocation = null;
        if (TheProperty != null && OfFaction != null)
        {
            targetLocation = CORE.Instance.Locations.Find(X => X.CurrentProperty == TheProperty && X.FactionInControl != null && X.FactionInControl.name == OfFaction.name);
        }
        else if(DialogWindowUI.Instance.GetDialogParameter("Location") != null)
        {
            targetLocation = (LocationEntity)DialogWindowUI.Instance.GetDialogParameter("Location");
        }

        if (targetLocation == null)
        {
            Debug.LogError("NO RELEVANT LOCATION FOR " + this.name);
            return;
        }

        if(targetLocation.EmployeesCharacters.Count == 0)
        {
            return;
        }

        Character targetChar = targetLocation.EmployeesCharacters[Random.Range(0, targetLocation.EmployeesCharacters.Count)];

        targetChar.StopDoingCurrentTask(false);
        ActionToDo.Execute(targetChar.TopEmployer, targetChar, targetChar.CurrentLocation);
    }
}
