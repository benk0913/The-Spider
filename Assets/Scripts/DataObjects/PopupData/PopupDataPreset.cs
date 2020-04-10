using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PopupDataPreset", menuName = "DataObjects/Popup/PopupDataPreset", order = 2)]
public class PopupDataPreset : ScriptableObject
{
    [TextArea(6,12)]
    public string Description;

    public Sprite Image;

    [SerializeField]
    public DialogDecisionAction OnOpenAction;


    public PopupDataPreset Clone()
    {
        PopupDataPreset newInstance = Instantiate(this);
        newInstance.name = this.name;

        return newInstance;
    }

}
