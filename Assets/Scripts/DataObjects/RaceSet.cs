using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Race Set", menuName = "DataObjects/Race Set", order = 2)]
public class RaceSet : ScriptableObject
{
    [SerializeField]
    public List<AgeSet> AgeSets = new List<AgeSet>();

    public AgeSet GetAgeSet(AgeTypeEnum age, bool fallback = true)
    {
        for(int i=0;i<AgeSets.Count;i++)
        {
            if(AgeSets[i].Age == age)
            {
                return AgeSets[i];
            }
        }

        if(fallback)
        {
            return AgeSets[0];
        }

        return null;
    }
}
