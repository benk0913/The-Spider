using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionCategory", menuName = "DataObjects/ActionCategory", order = 2)]
public class ActionCategory : ScriptableObject
{
    [TextArea(3,4)]
    public string Description;

    public Sprite Icon;
}
