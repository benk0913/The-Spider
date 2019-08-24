using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Gender Set", menuName = "DataObjects/Gender Set", order = 2)]
public class GenderSet : ScriptableObject
{
    [SerializeField]
    public GenderType Gender;

    [SerializeField]
    public VisualCharacteristic SkinColors;

    [SerializeField]
    public VisualCharacteristic HairColor;

    [SerializeField]
    public VisualCharacteristic Clothing;

    public VisualCharacteristic GetVCByName(string vcName)
    {
        VisualCharacteristic result = SkinColors.GetVCByName(vcName, false);

        if (result != null)
        {
            return result;
        }

        result = HairColor.GetVCByName(vcName, false);

        if (result != null)
        {
            return result;
        }

        result = Clothing.GetVCByName(vcName, false);

        if (result != null)
        {
            return result;
        }

        return null;
    }

}