using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTargetUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    public string Text;

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointAndClickTooltipUI.Instance.Show(Text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointAndClickTooltipUI.Instance.Hide();
    }

    void OnDisable()
    {
        PointAndClickTooltipUI.Instance.Hide();
    }
}
