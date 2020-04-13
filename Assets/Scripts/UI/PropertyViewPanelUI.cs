using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PropertyViewPanelUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Title;

    [SerializeField]
    TextMeshProUGUI Description;

    [SerializeField]
    Image IconImage;

    [SerializeField]
    Transform PossibleTierGrid;

    [SerializeField]
    Transform ActionsGrid;

    [SerializeField]
    PlotTypeUI PlotType;

    public void SetProperty(Property property)
    {
        Title.text = property.name;
        Description.text = property.Description;
        IconImage.sprite = property.Icon;

        PlotType.SetInfo(property.PlotType);

        ClearContainers();
        for (int i = 0; i < property.PropertyLevels.Count; i++)
        {
            GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject("PropertyLevelTierUI");
            tempObj.transform.SetParent(PossibleTierGrid, false);
            tempObj.transform.localScale = Vector3.one;
            tempObj.GetComponent<PropertyLevelTierUI>().SetInfo(property, property.PropertyLevels[i]);
        }

        foreach (PlayerAction action in property.UniquePlayerActions)
        {
            GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject("RightClickMenuItem");
            tempObj.transform.SetParent(ActionsGrid, false);
            tempObj.transform.localScale = Vector3.one;
            tempObj.GetComponent<RightClickMenuItemUI>().SetInfo(action.name, () => { }, action.Description, action.Icon, true);
        }
    }

    void ClearContainers()
    {
        while (PossibleTierGrid.childCount > 0)
        {
            PossibleTierGrid.GetChild(0).gameObject.SetActive(false);
            PossibleTierGrid.GetChild(0).transform.SetParent(transform);
        }

        while (ActionsGrid.childCount > 0)
        {
            ActionsGrid.GetChild(0).gameObject.SetActive(false);
            ActionsGrid.GetChild(0).transform.SetParent(transform);
        }
    }
}
