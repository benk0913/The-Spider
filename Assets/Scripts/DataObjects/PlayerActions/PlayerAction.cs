using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerAction", menuName = "DataObjects/PlayerActions/PlayerAction", order = 2)]
public class PlayerAction : ScriptableObject
{
    [TextArea(2, 3)]
    public string Description;

    public virtual void Execute(AgentInteractable target)
    {
        if (!CanDoAction(target))
        {
            GlobalMessagePrompterUI.Instance.Show("You cannot do this action!", 1f, Color.red);

            return;
        }
    }

    public virtual bool CanDoAction(AgentInteractable target)
    {
        return true;
    }
}
