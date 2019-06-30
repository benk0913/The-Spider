using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchasePlotPanelUI : MonoBehaviour
{
    public const string PROPERTY_BUTTON_PREFAB = "PurchasablePropertyItemUI";

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
    Transform PropertySelectionGrid;

    [SerializeField]
    GameObject SelectPropertyPanel;

    [SerializeField]
    GameObject IdlePanel;

    [SerializeField]
    Button BuyButton;

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

        GameClock.Instance.OnTurnPassed.AddListener(RefreshUI);

        RefreshUI();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
        GameClock.Instance.OnTurnPassed.RemoveListener(RefreshUI);
    }

    void RefreshUI()
    {
        BuyButton.interactable = (CurrentPlot.Price <= CORE.PC.Gold);
    }

    public void ShowPropertySelection()
    {
        SelectPropertyPanel.SetActive(true);
        IdlePanel.SetActive(false);

        ClearPropertySelectionGrid();

        GameObject tempPropertyButton;
        for(int i=0;i<CORE.Instance.Database.Properties.Count;i++)
        {
            if(CORE.Instance.Database.Properties[i].PlotType == CurrentPlot.Type)
            {
                tempPropertyButton = ResourcesLoader.Instance.GetRecycledObject(PROPERTY_BUTTON_PREFAB);

                tempPropertyButton.transform.SetParent(PropertySelectionGrid, false);
                tempPropertyButton.transform.localScale = Vector3.one;
            }
        }
    }

    public void HidePropertySelection()
    {
        SelectPropertyPanel.gameObject.SetActive(false);
        IdlePanel.SetActive(true);
    }

    void ClearPropertySelectionGrid()
    {
        while(PropertySelectionGrid.childCount > 0)
        {
            PropertySelectionGrid.GetChild(0).gameObject.SetActive(false);
            PropertySelectionGrid.GetChild(0).SetParent(transform, false);
        }
    }

    public void PurchasePlot(Property property)
    {
        if(CORE.PC.Gold < CurrentPlot.Price)
        {
            //TODO NOT ENOUGH MONEY ALERT.
            return;
        }

        GameObject locationPrefab = ResourcesLoader.Instance.GetRecycledObject(property.LocationPrefab);

        locationPrefab.transform.position = CurrentPlot.transform.position;
        locationPrefab.transform.rotation = CurrentPlot.transform.rotation;

        locationPrefab.GetComponent<LocationEntity>().SetInfo(property);

        Destroy(CurrentPlot.gameObject);
        Hide();
    }
}
