using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlotInfoPanelUI : MonoBehaviour
{
    public static PlotInfoPanelUI Instance;

    PurchasableEntity CurrentPlot;

    [SerializeField]
    TextMeshProUGUI TypeText;

    [SerializeField]
    TextMeshProUGUI PriceText;

    [SerializeField]
    TextMeshProUGUI RevMultiText;

    [SerializeField]
    TextMeshProUGUI RiskMultiText;

    private void Start()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Select(PurchasableEntity plot)
    {
        CurrentPlot = plot;

        TypeText.text = plot.Type.ToString();
        PriceText.text = plot.Price.ToString();
        RevMultiText.text = plot.RevenueMultiplier.ToString()+"x";
        RiskMultiText.text = plot.RiskMultiplier.ToString() + "x";
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }


}
