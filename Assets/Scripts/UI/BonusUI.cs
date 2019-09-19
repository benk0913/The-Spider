using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusUI : MonoBehaviour
{
    public Bonus CurrentBonus;

    [SerializeField]
    private TextMeshProUGUI ContentText;

    [SerializeField]
    Image Icon;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    public void SetInfo(Bonus bonus)
    {
        CurrentBonus = bonus;

        if (CurrentBonus == null)
        {
            ContentText.text = "???";
            Icon.color = Color.clear;

            TooltipTarget.Text = "Unknown Bonus";
            return;
        }

        ContentText.text = CurrentBonus.Type.name + " - " + CurrentBonus.Value;
        Icon.sprite = CurrentBonus.Type.icon;
        Icon.color = Color.white;

        TooltipTarget.Text = CurrentBonus.Type.Description;
    }

}
