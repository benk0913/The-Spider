using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Property", menuName = "DataObjects/Property", order = 2)]
public class Property : ScriptableObject
{
    public List<PropertyLevel> PropertyLevels = new List<PropertyLevel>();

    public Sprite Icon;

    [System.Serializable]
    public class PropertyLevel
    {
        public int MaxEmployees;
        public int MaxActions;
        public int UpgradePrice;
        public int UpgradeLength;
    }
}
