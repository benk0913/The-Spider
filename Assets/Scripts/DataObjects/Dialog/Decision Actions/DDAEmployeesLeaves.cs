using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAEmployeesLeaves", menuName = "DataObjects/Dialog/Actions/DDAEmployeesLeaves", order = 2)]
public class DDAEmployeesLeaves : DialogDecisionAction
{
    public Property TheProperty;
    public Faction OfFaction;
    public string ModifierMessage = "Likes you for some reason...";
    public bool IsGuard;

    public override void Activate()
    {

        LocationEntity targetLocation = null;
        if (TheProperty != null && OfFaction != null)
        {
            targetLocation = CORE.Instance.Locations.Find(X => X.CurrentProperty == TheProperty && X.FactionInControl != null && X.FactionInControl.name == OfFaction.name);
        }
        else if (DialogWindowUI.Instance.GetDialogParameter("Location") != null)
        {
            targetLocation = (LocationEntity)DialogWindowUI.Instance.GetDialogParameter("Location");
        }

        if (targetLocation == null)
        {
            Debug.LogError("NO RELEVANT LOCATION FOR " + this.name);
            return;
        }

        if (targetLocation.EmployeesCharacters.Count == 0)
        {
            return;
        }

        List<Character> targetCHars = new List<Character>();

        if (IsGuard)
        {
            targetCHars.AddRange(targetLocation.GuardsCharacters);
        }
        else
        {
            targetCHars.AddRange(targetLocation.EmployeesCharacters);
        }

        if(targetCHars.Count <= 0)
        {
            return;
        }

        Character character = targetCHars[Random.Range(0, targetCHars.Count)];

        WarningWindowUI.Instance.Show(character.name + " has stopped working for your " + targetLocation.Name,null);
        character.StopWorkingForCurrentLocation();
    }
}
