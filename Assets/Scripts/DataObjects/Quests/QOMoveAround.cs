using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOMoveAround", menuName = "DataObjects/Quests/QuestObjectives/QOMoveAround", order = 2)]
public class QOMoveAround : QuestObjective
{
    bool MovedForward= false;
    bool MovedBackward = false;
    bool MovedLeft = false;
    bool MovedRight = false;

    public override bool Validate()
    {
        if(Input.GetKeyDown(InputMap.Map["MoveForward"]))
        {
            MovedForward = true;
        }

        if (Input.GetKeyDown(InputMap.Map["MoveBackward"]))
        {
            MovedBackward = true;
        }

        if (Input.GetKeyDown(InputMap.Map["MoveLeft"]))
        {
            MovedLeft = true;
        }

        if (Input.GetKeyDown(InputMap.Map["MoveRight"]))
        {
            MovedRight = true;
        }

        return MovedForward && MovedBackward && MovedLeft && MovedRight;
    }
}