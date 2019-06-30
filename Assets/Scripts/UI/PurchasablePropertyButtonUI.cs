using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchasablePropertyButtonUI : MonoBehaviour
{
    [SerializeField]
    Image ImageFrame;

    Property CurrentProperty;

    PurchasePlotPanelUI ParentContainer;

    public void SetInfo(Property property, PurchasePlotPanelUI parentContainer)
    {
        ImageFrame.sprite = property.Icon;
        CurrentProperty = property;
        ParentContainer = parentContainer;
    }

    public void Select()
    {
        ParentContainer.PurchasePlot(CurrentProperty);
    }
}
