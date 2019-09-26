
using UnityEngine;
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
}

public class DescribedAction
{
    public string Key;
    public string Description;
    public UnityAction Action;
    public bool Interactable;
    public Sprite Icon;

    public DescribedAction(string key, UnityAction action, string description = "Possible Action", Sprite icon = null,bool interactable = false)
    {
        this.Key = key;
        this.Action = action;
        this.Description = description;
        this.Interactable = interactable;
        this.Icon = icon;
    }
}
