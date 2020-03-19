using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PropertyLevelTierUI : MonoBehaviour
{
    public TextMeshProUGUI TierTitle;
    public TextMeshProUGUI TierLevel;
    public TextMeshProUGUI TierMaxEmployees;
    public TextMeshProUGUI TierMaxGuards;
    public TextMeshProUGUI TierMaxPrisoners;
    public TextMeshProUGUI TierMaxInventory;

    public Transform ActionsUnlockedContainer;
    public Transform PossibleMerchContainer;

    Property CurrentProperty;
    Property.PropertyLevel CurrentLevel;

    public void SetInfo(Property property, Property.PropertyLevel level)
    {
        CurrentProperty = property;
        CurrentLevel = level;

        int currentLevelIndex = CurrentProperty.PropertyLevels.IndexOf(CurrentLevel);

        TierTitle.text = CurrentLevel.LevelName;
        TierLevel.text = "Level: "+ currentLevelIndex;

        TierMaxEmployees.gameObject.SetActive(CurrentLevel.MaxEmployees > 0);
        TierMaxEmployees.text = CurrentLevel.MaxEmployees.ToString();

        TierMaxGuards.text = CurrentLevel.MaxGuards.ToString();
        TierMaxGuards.gameObject.SetActive(CurrentLevel.MaxGuards > 0);

        TierMaxPrisoners.text = CurrentLevel.MaxPrisoners.ToString();
        TierMaxPrisoners.gameObject.SetActive(CurrentLevel.MaxPrisoners > 0);

        TierMaxInventory.text = CurrentLevel.InventoryCap.ToString();
        TierMaxInventory.gameObject.SetActive(CurrentLevel.InventoryCap > 0);

        ClearContainers();

        GameObject tempObj;
        foreach(Item item in CurrentLevel.PossibleMerchantise)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject("ItemUI");
            tempObj.transform.SetParent(PossibleMerchContainer, false);
            tempObj.transform.localScale = Vector3.one;
            tempObj.GetComponent<ItemUI>().SetInfo(item);
        }


        int previousMaxActions = currentLevelIndex == 0 ? -1 : CurrentProperty.PropertyLevels[currentLevelIndex-1].MaxActions-1;
        int currentMaxActions = CurrentProperty.PropertyLevels[currentLevelIndex].MaxActions-1;

        for (int i=0;i<CurrentProperty.Actions.Count;i++)
        {
            if (i > previousMaxActions && i == currentMaxActions)
            {
                tempObj = ResourcesLoader.Instance.GetRecycledObject("ActionUI");
                tempObj.transform.SetParent(PossibleMerchContainer, false);
                tempObj.transform.localScale = Vector3.one;
                tempObj.GetComponent<ActionUI>().SetInfo(null, CurrentProperty.Actions[i]);
            }
        }
    }

    void ClearContainers()
    {
        while (ActionsUnlockedContainer.childCount > 0)
        {
            ActionsUnlockedContainer.GetChild(0).gameObject.SetActive(false);
            ActionsUnlockedContainer.GetChild(0).SetParent(transform);
        }

        while (PossibleMerchContainer.childCount > 0)
        {
            PossibleMerchContainer.GetChild(0).gameObject.SetActive(false);
            PossibleMerchContainer.GetChild(0).SetParent(transform);
        }
    }
}
