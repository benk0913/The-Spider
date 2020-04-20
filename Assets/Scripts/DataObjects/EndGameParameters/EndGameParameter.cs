using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EndGameParameter", menuName = "DataObjects/EndGameParameter/EndGameParameter", order = 2)]
public class EndGameParameter : ScriptableObject
{
    public string Title;

    public Sprite icon;

    [TextArea(2,3)]
    public string Description;

    public virtual string GetValue()
    {
        return "";
    }

    public virtual Sprite GetIcon()
    {
        return icon;
    }
}

[System.Serializable]
public class EndgameParamInstance
{
    public string Key;
    public Sprite Icon;

    public string Value1;
    public string Value2;
}