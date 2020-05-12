using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KnownInstanceUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Title;

    [SerializeField]
    Image EyeIcon;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    public void SetInfo(string title, string description, bool isKnown)
    {
        Title.text = (isKnown ? "<color=#205B05> " : "<color=#9A0000>") + Regex.Replace(title, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0") + "</color>";

        TooltipTarget.SetTooltip(description + (isKnown ? "\n \n  (Known)" : " \n \n (Unknown)"));

        EyeIcon.enabled = isKnown;
    }
}
