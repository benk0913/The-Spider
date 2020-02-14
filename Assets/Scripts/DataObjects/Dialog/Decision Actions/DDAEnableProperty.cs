using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAEnableProperty", menuName = "DataObjects/Dialog/Actions/DDAEnableProperty", order = 2)]
public class DDAEnableProperty : DialogDecisionAction
{
    public string locationName;

    public bool ActiveState = true;

    public override void Activate()
    {
        LocationEntity location = CORE.Instance.Locations.Find(x => x.name == locationName);

        
        if(location == null)
        {
            Debug.LogError("Couldn't find location!");
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
