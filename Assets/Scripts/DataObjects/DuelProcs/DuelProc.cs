using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DuelProc", menuName = "DataObjects/DuelProcs/DuelProc", order = 2)]
public class DuelProc : ScriptableObject
{
    public DuelProcType Type;

    [TextArea(3,6)]
    public string Description;

    public float Chance = 1f;
    
    public virtual void Execute()
    {
        
    }

    public bool RollChance()
    {
        return Random.Range(0f, 1f) < Chance;
    }
}
