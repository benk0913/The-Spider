using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOLookAround", menuName = "DataObjects/Quests/QuestObjectives/QOLookAround", order = 2)]
public class QOLookAround : QuestObjective
{
    bool LookedLeftRight = false;
    bool LookedUpDown = false;

    public override bool Validate()
    {
        if(Mathf.Abs(Input.GetAxis("Mouse X")) > 1f)
        {
            LookedLeftRight = true;
        }

        if (Mathf.Abs(Input.GetAxis("Mouse Y")) > 1f)
        {
            LookedUpDown = true;
        }

        return LookedLeftRight && LookedUpDown;
    }
}