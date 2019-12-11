using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlLocationPanelUI : MonoBehaviour
{

    public const float PORTRAITS_SPACING = 5f;
    public const int PORTRAITS_MAX_IN_ROW = 5;

    [SerializeField]
    TextMeshProUGUI LocationTitle;

    [SerializeField]
    Transform RanksContainer;

    [SerializeField]
    GameObject UpgradeInProgressPanel;

    [SerializeField]
    GameObject UpgradeButton;

    [SerializeField]
    GameObject RebrandButton;

    [SerializeField]
    TextMeshProUGUI UpgradeLengthText;

    [SerializeField]
    TextMeshProUGUI UpgradePriceText;

    [SerializeField]
    Image UpgradeFillImage;

    [SerializeField]
    PortraitUIEmployee OwnerPortrait;

    [SerializeField]
    Transform EmployeeGrid;

    [SerializeField]
    Transform ActionGrid;

    [SerializeField]
    Transform GuardsGrid;

    [SerializeField]
    Transform PrisonersGrid;

    [SerializeField]
    Image RecruitingPanel;

    [SerializeField]
    GameObject RebrandBlockedSymbol;

    [SerializeField]
    LocationPortraitUI LocationPortrait;

    [SerializeField]
    Transform InventoryContainer;

    [SerializeField]
    GameObject BuyPanel;

    [SerializeField]
    TextMeshProUGUI BuyPriceText;

    [SerializeField]
    PlotTypeUI BuyPanelPlotType;

    LocationEntity CurrentLocation;

    public void Select(LocationEntity location)
    {
        Deselect();

        CurrentLocation = location;
        CurrentLocation.SetSelected();
        CurrentLocation.StateUpdated.AddListener(RefreshUI);

        //MapViewManager.Instance.FocusOnEntity(CurrentLocation.transform);

        RefreshUI();
    }

    public void Deselect()
    {
        if (CurrentLocation == null)
        {
            return;
        }

        CurrentLocation.SetDeselected();
        CurrentLocation.StateUpdated.RemoveListener(RefreshUI);
        CurrentLocation = null;
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        CORE.Instance.SubscribeToEvent("PassTimeComplete", OnTurnPassed);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
        CORE.Instance.UnsubscribeFromEvent("PassTimeComplete", OnTurnPassed);
        Deselect();
    }

    void OnTurnPassed()
    {
        RefreshPortraits();
    }

    public void RefreshUI()
    {
        RebrandBlockedSymbol.gameObject.SetActive(CurrentLocation.CurrentProperty.PlotType == CORE.Instance.Database.UniquePlotType);
        LocationTitle.text = CurrentLocation.Name;
        LocationPortrait.SetLocation(CurrentLocation);

        RefreshPortraits();

        RefreshRanks();

        RefreshUpgradeState();

        RefreshActions();

        UpgradeButton.gameObject.SetActive(
            CurrentLocation.IsOwnedByPlayer 
            && !CurrentLocation.IsUpgrading
            && CurrentLocation.CurrentProperty.PropertyLevels.Count > CurrentLocation.Level);

        RebrandButton.gameObject.SetActive(CurrentLocation.IsOwnedByPlayer);

        RefreshInventory();

        if(CurrentLocation.IsBuyable)
        {
            BuyPanel.gameObject.SetActive(true);
            BuyPriceText.text = CurrentLocation.LandValue + "c";
            BuyPanelPlotType.SetInfo(CurrentLocation.CurrentProperty.PlotType);
        }
        else
        {
            BuyPanel.gameObject.SetActive(false);
        }
    }

    void RefreshActions()
    {
        ClearActionsContainer();

        ActionUI tempActionUI;
        for (int i = 0; i < CurrentLocation.CurrentProperty.Actions.Count; i++)
        {
            tempActionUI = ResourcesLoader.Instance.GetRecycledObject("ActionUI").GetComponent<ActionUI>();
            tempActionUI.transform.SetParent(ActionGrid, false);
            tempActionUI.transform.localScale = Vector3.one;

            if (CurrentLocation.CurrentProperty.PropertyLevels[CurrentLocation.Level - 1].MaxActions > i)
            {
                tempActionUI.SetInfo(CurrentLocation, CurrentLocation.CurrentProperty.Actions[i]);
            }
            else
            {
                tempActionUI.SetInfo(CurrentLocation, null);
            }

            if(tempActionUI.CurrentAction == CurrentLocation.CurrentAction)
            {
                tempActionUI.SetSelected();
            }
            else
            {
                tempActionUI.SetDeselected();
            }
        }
    }

    void RefreshRanks()
    {
        for (int i = 0; i < RanksContainer.childCount; i++)
        {
            RanksContainer.GetChild(i).gameObject.SetActive(CurrentLocation.Level > i);
        }
    }

    void RefreshPortraits()
    {
        float productivity = 0f;
        if (CurrentLocation.OwnerCharacter != null)
        {
            productivity =
                CurrentLocation.OwnerCharacter.GetBonus(CurrentLocation.CurrentProperty.ManagementBonus).Value
                /
                CurrentLocation.OwnerCharacter.PropertiesOwned.Count;
        }

        OwnerPortrait.SetCharacter(CurrentLocation.OwnerCharacter, CurrentLocation, false);


        ClearEmployeeContainer();

        PortraitUIEmployee tempPortrait;
        for (int i = 0; i < CurrentLocation.CurrentProperty.PropertyLevels[CurrentLocation.Level - 1].MaxEmployees; i++)
        {
            tempPortrait = ResourcesLoader.Instance.GetRecycledObject("PortraitUIEmployee").GetComponent<PortraitUIEmployee>();
            tempPortrait.transform.SetParent(EmployeeGrid,false);
            tempPortrait.transform.localScale = Vector3.one;
            
            if (CurrentLocation.EmployeesCharacters.Count > i)
            {
                tempPortrait.SetCharacter(CurrentLocation.EmployeesCharacters[i], CurrentLocation, false);
            }
            else
            {
                tempPortrait.SetCharacter(null, CurrentLocation, true);
            }
        }


        ClearGuardsContainer();

        for (int i = 0; i < CurrentLocation.CurrentProperty.PropertyLevels[CurrentLocation.Level - 1].MaxGuards; i++)
        {
            tempPortrait = ResourcesLoader.Instance.GetRecycledObject("PortraitUIEmployee").GetComponent<PortraitUIEmployee>();
            tempPortrait.transform.SetParent(GuardsGrid, false);
            tempPortrait.transform.localScale = Vector3.one;
            
            if (CurrentLocation.GuardsCharacters.Count > i)
            {
                tempPortrait.SetCharacter(CurrentLocation.GuardsCharacters[i], CurrentLocation, false, true);
            }
            else
            {
                tempPortrait.SetCharacter(null, CurrentLocation, true, true);
            }
        }


        ClearPrisonersContainer();
        PortraitUI tempRegularPortrait; 
        for (int i = 0; i < CurrentLocation.CurrentProperty.PropertyLevels[CurrentLocation.Level - 1].MaxPrisoners; i++)
        {
            tempRegularPortrait = ResourcesLoader.Instance.GetRecycledObject("PortraitUI").GetComponent<PortraitUI>();
            tempRegularPortrait.transform.SetParent(PrisonersGrid, false);
            tempRegularPortrait.transform.localScale = Vector3.one;

            if (CurrentLocation.PrisonersCharacters.Count > i)
            {
                tempRegularPortrait.SetCharacter(CurrentLocation.PrisonersCharacters[i], CurrentLocation);
            }
            else
            {
                tempRegularPortrait.SetCharacter(null);
            }
        }
    }

    public void RefreshInventory()
    {
        while (InventoryContainer.childCount > 0)
        {
            InventoryContainer.GetChild(0).gameObject.SetActive(false);
            InventoryContainer.GetChild(0).SetParent(transform);
        }

        foreach (Item item in CurrentLocation.Inventory)
        {
            GameObject itemObj = ResourcesLoader.Instance.GetRecycledObject("ItemUI");
            itemObj.transform.SetParent(InventoryContainer, false);
            itemObj.transform.localScale = Vector3.one;
            itemObj.GetComponent<ItemUI>().SetInfo(item, CurrentLocation);
        }
    }

    void ClearEmployeeContainer()
    {
        while (EmployeeGrid.childCount > 0)
        {
            EmployeeGrid.GetChild(0).gameObject.SetActive(false);
            EmployeeGrid.GetChild(0).SetParent(transform);
        }
    }

    void ClearGuardsContainer()
    {
        while (GuardsGrid.childCount > 0)
        {
            GuardsGrid.GetChild(0).gameObject.SetActive(false);
            GuardsGrid.GetChild(0).SetParent(transform);
        }
    }

    void ClearPrisonersContainer()
    {
        while (PrisonersGrid.childCount > 0)
        {
            PrisonersGrid.GetChild(0).gameObject.SetActive(false);
            PrisonersGrid.GetChild(0).SetParent(transform);
        }
    }

    void ClearActionsContainer()
    {
        while (ActionGrid.childCount > 0)
        {
            ActionGrid.GetChild(0).gameObject.SetActive(false);
            ActionGrid.GetChild(0).SetParent(transform);
        }
    }

    void RefreshUpgradeState()
    {
        if (CurrentLocation.IsUpgrading)
        {
            UpgradeInProgressPanel.SetActive(true);
            LocationPortrait.gameObject.SetActive(false);

            GameClock.GameTimeLength upgradeLength = new GameClock.GameTimeLength(CurrentLocation.CurrentUpgradeLength);

            UpgradeLengthText.text = "<color=black>Ready In:</color>\n";
            UpgradeLengthText.text += ((GameClock.GameTime)upgradeLength.DayTime) + "\n";
            if (upgradeLength.Days > 0)
            {
                UpgradeLengthText.text += upgradeLength.Days.ToString()
                    + ((upgradeLength.Days == 1) ? " day from \n now..." : " days from \n now...");
            }

            UpgradeFillImage.fillAmount =
                ((float)CurrentLocation.CurrentUpgradeLength) 
                /
                ((float)CurrentLocation.CurrentProperty.PropertyLevels[CurrentLocation.Level].UpgradeLength);
        }
        else
        {
            UpgradeInProgressPanel.SetActive(false);
            LocationPortrait.gameObject.SetActive(true);

            if (CurrentLocation.CurrentProperty.PropertyLevels.Count > CurrentLocation.Level)
            {
                if (CurrentLocation.CurrentProperty.PropertyLevels.Count > CurrentLocation.Level)
                {
                    UpgradePriceText.text = CurrentLocation.CurrentProperty.PropertyLevels[CurrentLocation.Level].UpgradePrice.ToString();
                }
            }
        }
    }


    public void PurchaseUpgrade()
    {
        CurrentLocation.PurchaseUpgrade();
    }

    public void CancelUpgrade()
    {
        CurrentLocation.CancelUpgrade();
    }

    public void ShowRebrandWindow()
    {
        RebrandWindowUI.Instance.Show(CurrentLocation);
    }

    public void BuyLocation()
    {
        LocationEntity location = CurrentLocation;

        SelectAgentWindowUI.Instance.Show(
               (Character character) => 
               {
                   FailReason failure = location.PurchasePlot(CORE.PC, character);

                   if(failure != null)
                   {
                       GlobalMessagePrompterUI.Instance.Show(failure.Key, 1f, Color.red);
                   }
               }
               , (Character charInQuestion) => { return charInQuestion.TopEmployer == CORE.PC 
                   && charInQuestion != CORE.PC 
                   && charInQuestion.Age > 15 
                   && charInQuestion.IsAgent; });

        
    }
}
