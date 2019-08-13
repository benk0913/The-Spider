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

        ContentText.text = CurrentBonus.Type.name + " - " + CurrentBonus.Value;
        Icon.sprite = CurrentBonus.Type.icon;

        TooltipTarget.Text = CurrentBonus.Type.Description;
    }

}
