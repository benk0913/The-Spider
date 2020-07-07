using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAAchievement", menuName = "DataObjects/Dialog/Actions/DDAAchievement", order = 2)]
public class DDAAchievement : DialogDecisionAction
{
    public string AchievementKey;

    public override void Activate()
    {
        CORE.Instance.WinAchievement("ACH_"+AchievementKey);
    }
}
