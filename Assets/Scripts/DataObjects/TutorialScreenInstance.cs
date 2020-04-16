using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialScreenInstance", menuName = "DataObjects/TutorialScreenInstance", order = 2)]
public class TutorialScreenInstance : ScriptableObject
{
    public Sprite Image;
    public bool WasSeen = false;
    public string NextScreenKey;

    public TutorialScreenInstance Clone()
    {
        TutorialScreenInstance clone = Instantiate(this);
        clone.name = this.name;

        return clone;
    }
}