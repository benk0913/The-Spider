
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
    Naval,
    Unique
}

public static class DEF
{
    public const string LOCATION_PREFAB = "Location";
    public const string RIGHT_CLICK_DROPDOWN_ITEM = "RightClickMenuItem";
    public const string PORTRAIT_PREFAB = "PortraitUI";
    public const string ACTION_PREFAB   = "ActionUI";
    public const string HOVER_PANEL_PREFAB = "HoverPanelUI";
    public const string UNIQUE_PLOT     = "Unique";
    public const string LOCATION_MARKER_PREFAB = "LocationMarker";
    //TODO Move all game consts to here.
}

public class DescribedAction
{
    public string Key;
    public string Description;
    public UnityAction Action;
    public bool Interactable;

    public DescribedAction(string key, UnityAction action, string description = "Possible Action", bool interactable = false)
    {
        this.Key = key;
        this.Action = action;
        this.Description = description;
        this.Interactable = interactable;
    }
}
