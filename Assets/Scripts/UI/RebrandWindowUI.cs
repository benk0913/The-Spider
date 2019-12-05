using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RebrandWindowUI : MonoBehaviour
{
    public static RebrandWindowUI Instance;

    [SerializeField]
    TextMeshProUGUI Title;

    [SerializeField]
    TextMeshProUGUI Description;

    [SerializeField]
    Image IconImage;

    [SerializeField]
    Transform PossibleTierGrid;

    [SerializeField]
    PlotTypeUI PlotType;

    [SerializeField]
    GameObject CurrentPropertyPointer;

    LocationEntity CurrentLocation;

    public int CurrentIndex = 0;

    private void Awake()
    {
        Instance = this;

        Hide();
    }

    public void Show(LocationEntity currentLocation)
    {
        CurrentIndex = 0;
        CurrentLocation = currentLocation;

        this.gameObject.SetActive(true);

        SetProperty(CurrentLocation.CurrentProperty);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Next()
    {
        CurrentIndex++;

        if(CurrentIndex >= CORE.PlayerFaction.FactionProperties.Length)
        {
            CurrentIndex = 0;
        }

        if(CORE.PlayerFaction.FactionProperties[CurrentIndex].PlotType != CurrentLocation.CurrentProperty.PlotType)
        {
            Next();
            return;
        }

        SetProperty(CORE.PlayerFaction.FactionProperties[CurrentIndex]);
    }

    public void Previous()
    {
        CurrentIndex--;

        if (CurrentIndex < 0)
        {
            CurrentIndex = CORE.PlayerFaction.FactionProperties.Length - 1;
        }

        if (CORE.PlayerFaction.FactionProperties[CurrentIndex].PlotType != CurrentLocation.CurrentProperty.PlotType)
        {
            Previous();
            return;
        }

        SetProperty(CORE.PlayerFaction.FactionProperties[CurrentIndex]);
    }

    public void SetProperty(Property property)
    {
        CurrentPropertyPointer.gameObject.SetActive(property == CurrentLocation.CurrentProperty);

        Title.text = property.name;
        Description.text = property.Description;
        IconImage.sprite = property.Icon;

        ClearPossibleActionsContainer();
        for(int i=0;i<property.PropertyLevels.Count;i++)
        {
            GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject("PropertyLevelTierUI");
            tempObj.transform.SetParent(PossibleTierGrid, false);
            tempObj.transform.localScale = Vector3.one;
            tempObj.GetComponent<PropertyLevelTierUI>().SetInfo(property, property.PropertyLevels[i]);
        }

        PlotType.SetInfo(property.PlotType);
    }

    void ClearPossibleActionsContainer()
    {
        while(PossibleTierGrid.childCount > 0)
        {
            PossibleTierGrid.GetChild(0).gameObject.SetActive(false);
            PossibleTierGrid.GetChild(0).transform.SetParent(transform);
        }
    }

    public void ConfirmProperty()
    {
        CurrentLocation.Rebrand(CORE.PC, CORE.PlayerFaction.FactionProperties[CurrentIndex]);
        Hide();
    }
}
