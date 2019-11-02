﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTargetUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    public string Text;

    public List<TooltipBonus> Bonuses;

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointAndClickTooltipUI.Instance.Show(Text, Bonuses);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointAndClickTooltipUI.Instance.Hide();
    }

    public void SetTooltip(string text, List<TooltipBonus> tooltipBonuses = null)
    {
        this.Text = text;

        this.Bonuses = tooltipBonuses;
    }

    void OnDisable()
    {
        if(PointAndClickTooltipUI.Instance == null)
        {
            return;
        }

        PointAndClickTooltipUI.Instance.Hide();
    }
}

public class TooltipBonus
{
    public string Text;
    public Sprite Icon;

    public TooltipBonus(string text ,Sprite icon)
    {
        this.Text = text;
        this.Icon = icon;
    }
}
