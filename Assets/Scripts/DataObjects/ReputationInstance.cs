using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReputationInstance", menuName = "DataObjects/ReputationInstance", order = 2)]
public class ReputationInstance : ScriptableObject
{
    public Color color;

    public int AgentRelationModifier = 0;
    public int RecruitExtraCost = 0;
}
