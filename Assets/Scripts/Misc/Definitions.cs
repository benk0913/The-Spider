
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

    //TODO Move all game consts to here.
}

