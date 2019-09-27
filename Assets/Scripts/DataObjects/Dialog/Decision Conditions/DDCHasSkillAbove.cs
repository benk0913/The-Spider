using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDCHasSkillAbove", menuName = "DataObjects/Dialog/Conditions/DDCHasSkillAbove", order = 2)]
public class DDCHasSkillAbove : DialogDecisionCondition
{
    public string ParameterKey = "Actor";
    public BonusType Skill;
    public int AboveNumber = 1;

    public override bool CheckCondition()
    {
        Character character = (Character)DialogWindowUI.Instance.GetDialogParameter(ParameterKey);

        if(character.GetBonus(Skill).Value <= AboveNumber)
        {
            if (Inverted)
            {
                return base.CheckCondition();
            }

            return false;
        }

        return base.CheckCondition();
    }
}
