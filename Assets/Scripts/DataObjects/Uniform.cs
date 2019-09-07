using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Uniform", menuName = "DataObjects/Uniform", order = 2)]
public class Uniform : ScriptableObject
{
    public VisualCharacteristic MaleClothing;
    public VisualCharacteristic FemaleClothing;
}
