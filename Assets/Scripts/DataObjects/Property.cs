using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Property", menuName = "DataObjects/Property", order = 2)]
public class Property : ScriptableObject
{
    [TextArea(2,3)]
    public string Description;

    public List<PropertyLevel> PropertyLevels = new List<PropertyLevel>();

    public List<PropertyAction> Actions = new List<PropertyAction>();

    public List<PropertyTrait> Traits = new List<PropertyTrait>();

    public Sprite Icon;

    public GameObject FigurePrefab;
    public GameObject HoverPrefab;

    public Material MaterialOverride;

    public int RecruitingGenderType = -1;
    public int MinAge = 0;
    public int MaxAge = 120;

    public PlotType PlotType;

    public BonusType ManagementBonus;

    public bool isPublicArea = false;

    [System.Serializable]
    public class PropertyLevel
    {
        public int MaxEmployees;
        public int MaxActions;
        public int UpgradePrice;
        public int UpgradeLength;
        public int RecruitmentLength;
    }


    [System.Serializable]
    public class PropertyAction
    {
        public string Name;

        [TextArea(2,3)]
        public string Description;

        public Sprite Icon;

        public int GoldGeneratedMin = 1;
        public int GoldGeneratedMax = 2;

        public AgentAction WorkAction;
    }
}
