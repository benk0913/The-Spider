using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DialogDecisionCondition", menuName = "DataObjects/Dialog/Conditions/DialogDecisionCondition", order = 2)]
public class DialogDecisionCondition : ScriptableObject
{
    public bool Inverted = false;

    public virtual bool CheckCondition()
    {
        return true;
    }
}
