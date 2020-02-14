using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAActivateMapGameobject", menuName = "DataObjects/Dialog/Actions/DDAActivateMapGameobject", order = 2)]
public class DDAActivateMapGameobject : DialogDecisionAction
{
    public string ObjectName;

    public bool ActiveState = true;

    public override void Activate()
    {
        GameObject.Find(ObjectName).SetActive(ActiveState);
    }
}
