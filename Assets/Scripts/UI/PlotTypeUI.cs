using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlotTypeUI : MonoBehaviour
{
    [SerializeField]
    Image IconImage;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    public PlotType CurrentType;

    public void SetInfo(PlotType type)
    {
        CurrentType = type;
        RefreshUI();
    }

    void RefreshUI()
    {
        IconImage.sprite = CurrentType.Icon;
        TooltipTarget.Text = CurrentType.Description;
    }
}
