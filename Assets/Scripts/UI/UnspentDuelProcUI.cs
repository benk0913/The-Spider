using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnspentDuelProcUI : MonoBehaviour
{
    DuelProc Proc;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    [SerializeField]
    Image IconImage;

    public void SetInfo(DuelProc proc, Color color)
    {
        this.Proc = proc;

        TooltipTarget.SetTooltip(Proc.Description);
        IconImage.sprite = proc.SideIcon;
        IconImage.color = color;
    }
}
