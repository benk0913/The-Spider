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
        Character character = null;

        if (!string.IsNullOrEmpty(ParameterKey))
        {
            character = (Character)DialogWindowUI.Instance.GetDialogParameter(ParameterKey);
        }
        else
        {
            List<Character> charsInCommand = CORE.PC.CharactersInCommand;
            character = charsInCommand.Find(x => x.GetBonus(Skill).Value > AboveNumber);
        }

        if(character == null)
        {
            if (Inverted)
            {
                return base.CheckCondition();
            }

            return false;
        }

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
