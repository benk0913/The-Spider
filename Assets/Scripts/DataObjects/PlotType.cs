using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlotType", menuName = "DataObjects/PlotType", order = 2)]
public class PlotType : ScriptableObject
{
    public Sprite Icon;

    [TextArea(2,3)]
    public string Description;

    public Property BaseProperty;

    public GameObject BasePrefab;
}
