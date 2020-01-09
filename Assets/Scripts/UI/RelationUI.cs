using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelationUI : MonoBehaviour
{
    [SerializeField]
    Image imageIcon;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    [SerializeField]
    TextMeshProUGUI RelationCountText;

    Character CurrentCharacter;
    Character TargetCharacter;

    public void SetInfo(Character currentCharacter, Character targetCharacter)
    {
        this.CurrentCharacter = currentCharacter;
        this.TargetCharacter = targetCharacter;

        if(this.CurrentCharacter == null || this.TargetCharacter == null)
        {
            TooltipTarget.SetTooltip("Relations Unknown");
            imageIcon.sprite = ResourcesLoader.Instance.GetSprite("Indifferent");
            RelationCountText.text = "?";
            RelationCountText.color = Color.black;
            return;
        }

        int relationValue = CurrentCharacter.GetRelationsWith(TargetCharacter);
        TooltipTarget.SetTooltip(
            "<u> Opinion on " 
            + TargetCharacter.name + ": "
            + (relationValue >= 0 ? "<color=green>" : "<color=red>") 
            + relationValue
            + "</color> </u> \n");



        RelationsModifier[] modifiers = CurrentCharacter.GetRelationModifiers(TargetCharacter);

        TooltipTarget.Text += "<size=16>";

        foreach (RelationsModifier modifier in modifiers)
        {
            TooltipTarget.Text +=
                "\n "
                + modifier.Message
                + (modifier.Value >= 0 ? "<color=green> +" : "<color=red>")
                +  modifier.Value 
                +"</color>";
        }

        TooltipTarget.Text += "</size> \n Click to see this characters relations with others...";

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

        RelationCountText.text = relationValue.ToString();
        RelationCountText.color = relationValue >= 0 ? Color.green : Color.red;
    }

    public void OnClick()
    {
        if (this.CurrentCharacter == null || this.TargetCharacter == null)
        {
            return;
        }

        CharacterRelationsViewUI.Instance.Show(CurrentCharacter, null, null, CurrentCharacter.name + "'s Relations With Characters");
    }
}
