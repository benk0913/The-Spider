using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentInteractable : MonoBehaviour
{
    public virtual List<AgentAction> GetPossibleActions(Character forCharacter)
    {
        return null;
    }

    public virtual void ExecuteAgentAction(Character byCharacter, AgentAction action)
    {

    }

}
