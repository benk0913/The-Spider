using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SpecificBook", menuName = "DataObjects/SpecificBook", order = 2)]
public class SpecificBook : DialogDecisionAction
{
    public List<Sprite> Pages = new List<Sprite>();
}
