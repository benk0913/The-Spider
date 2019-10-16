using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleGate : MonoBehaviour
{
    public List<GateCondition> GateConditions = new List<GateCondition>();
    public List<UnityEvent> States = new List<UnityEvent>();

    public int CurrentState = 0;

    public void ToggleState()
    {
        foreach(GateCondition condition in GateConditions)
        {
            if(!condition.Unlocked)
            {
                return;
            }
        }

        CurrentState++;
        if(CurrentState >= States.Count)
        {
            CurrentState = 0;
        }

        States[CurrentState].Invoke();
    }
}

[System.Serializable]
public class GateCondition
{
    public PuzzleGate Gate;
    public int TargetState;
    public bool Unlocked
    {
        get
        {
            return Gate.CurrentState == TargetState;
        }
    }
} 