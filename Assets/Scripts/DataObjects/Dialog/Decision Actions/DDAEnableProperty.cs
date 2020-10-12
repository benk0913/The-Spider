using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAEnableProperty", menuName = "DataObjects/Dialog/Actions/DDAEnableProperty", order = 2)]
public class DDAEnableProperty : DialogDecisionAction
{
    public string LocationID;

    public bool ActiveState = true;

    public override void Activate()
    {
        LocationEntity location = CORE.Instance.Locations.Find(x => x.ID == LocationID);

        
        if(location == null)
        {
            Debug.LogError("Couldn't find location! "+LocationID);
            return;
        }

        if(ActiveState)
        {
            location.EnableProperty();
        }
        else
        {
            location.DisableProperty();
        }
    }
}
