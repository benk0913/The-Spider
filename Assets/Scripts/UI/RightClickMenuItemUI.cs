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
    TooltipTargetUI TooltipTarget;

    public void SetInfo(string title, UnityAction Action, string description = "")
    {
        Title.text = title;
        _Button.onClick.RemoveAllListeners();
        _Button.onClick.AddListener(Action);
        TooltipTarget.Text = description;
    }

    public void SetInfo(string title, UnityAction[] Actions, string description = "")
    {
        Title.text = title;
        _Button.onClick.RemoveAllListeners();

        foreach(UnityAction act in Actions)
            _Button.onClick.AddListener(act);

        TooltipTarget.Text = description;
    }
}
