using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PurchasablePlotUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI PriceText;
    int CurrentPrice;

    public void Initialize(int Price)
    {
        CurrentPrice = Price;
        PriceText.text = CurrentPrice.ToString();
    }

    public void PurchasePlot()
    {
        MapViewManager.Instance.PurchasePlot(this);
    }
}
