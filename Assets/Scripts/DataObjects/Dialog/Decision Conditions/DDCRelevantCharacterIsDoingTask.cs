using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDCRelevantCharacterIsDoingTask", menuName = "DataObjects/Dialog/Conditions/DDCRelevantCharacterIsDoingTask", order = 2)]
public class DDCRelevantCharacterIsDoingTask : DialogDecisionCondition
{
    public LongTermTask Task;

    public override bool CheckCondition()
    {
        if(DialogWindowUI.Instance.CurrentPiece.TargetCharacters == null || DialogWindowUI.Instance.CurrentPiece.TargetCharacters.Length == 0)
        {
            if (Inverted)
            {
                return base.CheckCondition();
            }

            return false;
        }

        Character target = DialogWindowUI.Instance.CurrentPiece.TargetCharacters[0];

        if (target.CurrentTaskEntity == null)
        {
            if (Inverted)
            {
                return base.CheckCondition();
            }

            return false;
        }

        if (target.CurrentTaskEntity.CurrentTask.name != Task.name)
        {
            if (Inverted)
            {
                return base.CheckCondition();
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
