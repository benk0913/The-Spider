using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RightClickMenuItemUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Title;

    [SerializeField]
    Button _Button;

    [SerializeField]
    Image Icon;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    public void SetInfo(string title, UnityAction Action, string description = "", Sprite icon = null, bool interactable = false)
    {
        Title.text = title;
        _Button.onClick.RemoveAllListeners();
        _Button.onClick.AddListener(Action);
        TooltipTarget.SetTooltip(title + " \n"+description);

        _Button.interactable = interactable;

        if(interactable == false)
        {
            Icon.color = new Color(Icon.color.r, Icon.color.g, Icon.color.b, 0.5f);
        }
        else
        {
            Icon.color = new Color(Icon.color.r, Icon.color.g, Icon.color.b, 1f);
        }

        this.Icon.sprite = icon;
    }

    public void SetInfo(string title, UnityAction[] Actions, string description = "", Sprite icon = null, bool interactable = false, List<TooltipBonus> tooltipBonuses = null)
    {
        Title.text = title;
        _Button.onClick.RemoveAllListeners();

        foreach(UnityAction act in Actions)
            _Button.onClick.AddListener(act);

        TooltipTarget.SetTooltip("<color=yellow>"+title+"</color>" + " \n" + description, tooltipBonuses);

        _Button.interactable = interactable;

        if (interactable == false)
        {
            Icon.color = new Color(Icon.color.r, Icon.color.g, Icon.color.b, 0.5f);
        }
        else
        {
            Icon.color = new Color(Icon.color.r, Icon.color.g, Icon.color.b, 1f);
        }

        this.Icon.sprite = icon;
    }
}
