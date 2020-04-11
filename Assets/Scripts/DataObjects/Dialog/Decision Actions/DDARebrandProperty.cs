using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDARebrandProperty", menuName = "DataObjects/Dialog/Actions/DDARebrandProperty", order = 2)]
public class DDARebrandProperty : DialogDecisionAction
{
    public string LocationID;
    public Property ToProperty;

    public override void Activate()
    {
        LocationEntity location = CORE.Instance.Locations.Find(x => x.ID == LocationID);


        if (location == null)
        {
            Debug.LogError("Couldn't find location!");
            return;
        }

        location.Rebrand(CORE.PC, ToProperty);
    }
}
