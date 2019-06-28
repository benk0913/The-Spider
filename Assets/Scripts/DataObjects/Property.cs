﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Property", menuName = "DataObjects/Property", order = 2)]
public class Property : ScriptableObject
{
    public List<PropertyLevel> PropertyLevels = new List<PropertyLevel>();

    public List<PropertyAction> Actions = new List<PropertyAction>();

    public Sprite Icon;

    public GameObject FigurePrefab;

    [System.Serializable]
    public class PropertyLevel
    {
        public int MaxEmployees;
        public int MaxActions;
        public int UpgradePrice;
        public int UpgradeLength;
    }


    [System.Serializable]
    public class PropertyAction
    {
        public string Name;

        public Sprite Icon;

        public int GoldGeneratedMin = 1;
        public int GoldGeneratedMax = 2;
    }
}
