using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameStatUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI TitleText;

    [SerializeField]
    TextMeshProUGUI ValueText;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    [SerializeField]
    Image IconImage;

    [SerializeField]
    PortraitUI Portrait;

    public void Show(string title = "", string description = "", Sprite icon = null, string value = "", Character character = null)
    {
        this.TitleText.text = title;
        this.ValueText.text = value;
        this.TooltipTarget.SetTooltip(description);

        if (IconImage != null)
        {
            this.IconImage.sprite = icon;
        }

        if(character != null)
        {
            this.Portrait.SetCharacter(character);
        }
    }
}
