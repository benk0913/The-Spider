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

}