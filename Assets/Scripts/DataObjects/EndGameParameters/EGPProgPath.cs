using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "EGPProgPath", menuName = "DataObjects/EndGameParameter/EGPProgPath", order = 2)]
public class EGPProgPath : EndGameParameter
{
    public List<EndgameParamInstance> Instances = new List<EndgameParamInstance>();

    public override string GetValue()
    {
        foreach (EndgameParamInstance instance in Instances)
        {
            instance.Value2 = CORE.Instance.TechTree.Find(x => x.name == instance.Value1).TotalPointsSpentOnBranch.ToString();
        }

        Instances = Instances.OrderByDescending(x => int.Parse(x.Value2)).ToList();

        return Instances[0].Key;
    }

    public override Sprite GetIcon()
    {
        foreach (EndgameParamInstance instance in Instances)
        {
            instance.Value2 = CORE.Instance.TechTree.Find(x => x.name == instance.Value1).TotalPointsSpentOnBranch.ToString();
        }

        Instances = Instances.OrderByDescending(x => int.Parse(x.Value2)).ToList();

        return Instances[0].Icon;
    }
}