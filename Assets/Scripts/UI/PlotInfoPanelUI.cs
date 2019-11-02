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

    [SerializeField]
    PlotTypeUI PlotTypeView;

    private void Start()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Select(PurchasableEntity plot)
    {
        CurrentPlot = plot;

        TypeText.text = plot.Type.name;
        PriceText.text = plot.Price.ToString();
        PlotTypeView.SetInfo(plot.Type);

        Show();
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void PurchasePlot()
    {
        GlobalMessagePrompterUI.Instance.Show("WORK IN PROGRESS");//TODO this...
    }

}
