using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FactionCityControlUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    Image PanelImage;

    [SerializeField]
    RectTransform Element;

    Faction CurrentFaction;
    float CurrentPrecent;

    public void SetInfo(Faction faction, float precent, float totalSize)
    {
        CurrentFaction = faction;
        CurrentPrecent = precent;

        PanelImage.color = CurrentFaction.FactionColor;

        Element.sizeDelta = new Vector2(totalSize * precent, Element.sizeDelta.y);
    }

    public void OnHover()
    {
        CityControlUI.Instance.ShowFactionLabel(CurrentFaction, CurrentPrecent);
    }

    public void OnUnhover()
    {
        CityControlUI.Instance.HideFactionLabel();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        FactionInfoUI.Instance.Show(CurrentFaction);
    }
}
