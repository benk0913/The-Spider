using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelationUI : MonoBehaviour
{
    [SerializeField]
    Image imageIcon;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    Character CurrentCharacter;
    Character TargetCharacter;

    public void SetInfo(Character currentCharacter, Character targetCharacter)
    {
        this.CurrentCharacter = currentCharacter;
        this.TargetCharacter = targetCharacter;

        int relationValue = CurrentCharacter.GetRelationsWith(TargetCharacter);
        TooltipTarget.Text = 
            "<u> Opinion of " 
            + TargetCharacter.name + ": "
            + (relationValue >= 0 ? "<color=green>" : "<color=red>") 
            + relationValue
            + "</color> </u> \n";


        RelationsModifier[] modifiers = CurrentCharacter.GetRelationModifiers(TargetCharacter);

        TooltipTarget.Text += "<size=12>";

        foreach (RelationsModifier modifier in modifiers)
        {
            TooltipTarget.Text +=
                "\n "
                + modifier.Message
                + (modifier.Value >= 0 ? "<color=green> +" : "<color=red>")
                +  modifier.Value 
                +"</color>";
        }

        TooltipTarget.Text += "</size>";

        if (relationValue > 5)
        {
            imageIcon.sprite = ResourcesLoader.Instance.GetSprite("Satisfied");
        }
        else if (relationValue < -5)
        {
            imageIcon.sprite = ResourcesLoader.Instance.GetSprite("Unsatisfied");
        }
        else
        {
            imageIcon.sprite = ResourcesLoader.Instance.GetSprite("Indifferent");
        }
    }
}
