using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerAction", menuName = "DataObjects/PlayerActions/PlayerAction", order = 2)]
public class PlayerAction : ScriptableObject
{
    [TextArea(2, 3)]
    public string Description;

    [SerializeField]
    public Sprite Icon;

    public virtual void Execute(Character requester, AgentInteractable target)
    {
        string reason;
        if (!CanDoAction(requester, target, out reason))
        {
            GlobalMessagePrompterUI.Instance.Show("You cannot do this action!", 1f, Color.red);

            return;
        }
    }

    public virtual bool CanDoAction(Character requester, AgentInteractable target, out string reason)
    {
        reason = "";
        return true;
    }
}
