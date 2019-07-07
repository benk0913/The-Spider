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

    public virtual void Execute(Character character, AgentInteractable target)
    {
        
    }
}
