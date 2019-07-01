
using UnityEngine.Events;

public enum GenderType
{
    Male,
    Female
}

[System.Serializable]
public enum AgeTypeEnum
{
    Baby,
    Child,
    Adult,
    Old
}

public enum PropType
{
    Wall,
    Floor,
    Decoration,
    Furniture,
    Carpet,
    BuildablePlot
}


[System.Serializable]
public enum Equation
{
    Above,
    AboveOrEquals,
    Equals,
    BelowOrEquals,
    Below
}

[System.Serializable]
public enum PurchasablePlotType
{
    Urban,
    Coastal,
    Naval
}

public static class DEF
{
    public const string LOCATION_PREFAB = "Location";
    public const string RIGHT_CLICK_DROPDOWN_ITEM = "RightClickMenuItem";
    //TODO Move all game consts to here.
}

public class KeyActionPair
{
    public string Key;
    public UnityAction Action;

    public KeyActionPair(string key, UnityAction action)
    {
        this.Key = key;
        this.Action = action;
    }
}
