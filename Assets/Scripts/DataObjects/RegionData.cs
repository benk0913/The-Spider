using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RegionData", menuName = "DataObjects/RegionData", order = 2)]
public class RegionData : ScriptableObject
{
    public string Name;

    public enum RegionTier
    {
        Cheap,
        Normal,
        Expensive,
        Rich
    }
}
