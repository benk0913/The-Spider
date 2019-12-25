using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOControlPrecentageOfCity", menuName = "DataObjects/Quests/QuestObjectives/QOControlPrecentageOfCity", order = 2)]
public class QOControlPrecentageOfCity : QuestObjective
{
    public float precentage;

    public override bool Validate()
    {
        if(ParentQuest.ForCharacter == null)
        {
            return false;
        }

        return CityControlUI.Instance.GetControlPrecentage(ParentQuest.ForCharacter.CurrentFaction) > precentage;
    }
    
}