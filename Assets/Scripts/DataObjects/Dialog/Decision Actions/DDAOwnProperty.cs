using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAOwnProperty", menuName = "DataObjects/Dialog/Actions/DDAOwnProperty", order = 2)]
public class DDAOwnProperty : DialogDecisionAction
{
    public string LocationID;

    public override void Activate()
    {
        LocationEntity location = CORE.Instance.Locations.Find(x => x.ID == LocationID);


        if (location == null)
        {
            Debug.LogError("Couldn't find location!");
            return;
        }

        CORE.PC.CharactersInCommand[0].StartOwningLocation(location);
    }
}
