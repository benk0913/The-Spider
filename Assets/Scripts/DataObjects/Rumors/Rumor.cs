using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rumor", menuName = "DataObjects/Rumor", order = 2)]
public class Rumor : ScriptableObject
{
    public string Title;

    [TextArea(2,6)]
    public string Description;
   
    public Sprite Icon;

    public string RelevantCharacterID;
}
