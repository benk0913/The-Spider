using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDCHasProperty", menuName = "DataObjects/Dialog/Conditions/DDCHasProperty", order = 2)]
public class DDCHasProperty : DialogDecisionCondition
{
    public Property PropertyType;

    public Faction RelevantFaction;

    public int MinimumEmployees = 0;

    public override bool CheckCondition()
    {
        Faction currentFaction = RelevantFaction;

        if(currentFaction == null)
        {
            currentFaction = CORE.PC.CurrentFaction;
        }

        LocationEntity targetLocation = CORE.Instance.Locations.Find(x => x.CurrentProperty == PropertyType  
        && x.FactionInControl != null 
        && x.FactionInControl.name == currentFaction.name 
        && x.EmployeesCharacters.Count >= MinimumEmployees);

        if(targetLocation == null)
        {
            if (Inverted)
            {
                return base.CheckCondition(); ;
            }

            return false;
        }

        if (Inverted)
        {
            return false;
        }

        return base.CheckCondition();
    }
}
