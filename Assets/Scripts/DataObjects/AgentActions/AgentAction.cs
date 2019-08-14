using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AgentAction", menuName = "DataObjects/AgentActions/AgentAction", order = 2)]
public class AgentAction : ScriptableObject
{
    public Sprite Icon;

    [TextArea(2,3)]
    public string Description;

    public List<Trait> RequiredTraits = new List<Trait>();

    public int MinimumAge = 0;

    public virtual void Execute(Character requester, Character character, AgentInteractable target)
    {
        if(!CanDoAction(requester, character, target))
        {
            GlobalMessagePrompterUI.Instance.Show("This character can not do this action! ", 1f, Color.red);

            return;
        }
    }

    public virtual bool CanDoAction(Character requester, Character character, AgentInteractable target)
    {
        if(character.Age < MinimumAge)
        {
            return false;
        }

        return true;
    }
}
