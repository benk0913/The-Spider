using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Image Icon;
    public TooltipTargetUI TooltipTarget;

    Item CurrentItem;

    public void SetInfo(Item item)
    {
        CurrentItem = item;

        RefreshUI();
    }

    void RefreshUI()
    {
        Icon.sprite = CurrentItem.Icon;
        TooltipTarget.Text = CurrentItem.Description;
    }
}
