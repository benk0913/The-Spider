using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Age Set", menuName = "DataObjects/Age Set", order = 2)]
public class AgeSet : ScriptableObject
{
    [SerializeField]
    public AgeTypeEnum Age;

    [SerializeField]
    public GenderSet Male;

    [SerializeField]
    public GenderSet Female;
}
