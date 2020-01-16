using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FavorDecision", menuName = "DataObjects/FavorDecision", order = 2)]
public class FavorDecision : ScriptableObject
{
    [TextArea(6,6)]
    public string Description;
    public Sprite Icon;
    public int FavorCost;
    public int FavorDuration;
    public AgentAction ActionToExecute;
}
