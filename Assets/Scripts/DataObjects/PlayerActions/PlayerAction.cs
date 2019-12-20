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

    public ActionCategory Category = null;

    public TechTreeItem TechRequired;

    public virtual void Execute(Character requester, AgentInteractable target)
    {

        FailReason reason;
        if (!CanDoAction(requester, target, out reason))
        {
            GlobalMessagePrompterUI.Instance.Show("You cannot do this action!", 1f, Color.red);

            return;
        }
    }

    public virtual bool CanDoAction(Character requester, AgentInteractable target, out FailReason reason)
    {
        reason = null;

        if (TechRequired != null)
        {
            TechTreeItem techInstance = CORE.Instance.TechTree.Find(x => x.name == TechRequired.name);

            if (techInstance != null && !techInstance.IsResearched)
            {
                return false;
            }
        }

        return true;
    }
}

public class FailReason
{
    public string Key;
    public int Value;

    public FailReason(string key = "", int value = 0)
    {
        this.Key = key;
        this.Value = value;
    }
}
