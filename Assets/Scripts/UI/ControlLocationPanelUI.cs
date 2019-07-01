﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlLocationPanelUI : MonoBehaviour
{

    public const float PORTRAITS_SPACING = 5f;
    public const int PORTRAITS_MAX_IN_ROW = 5;
    public const string PORTRAIT_PREFAB = "PortraitUI";
    public const string ACTION_PREFAB   = "ActionUI";

    [SerializeField]
    Image LocationIcon;

    [SerializeField]
    TextMeshProUGUI LocationTitle;

    [SerializeField]
    Transform RanksContainer;

    [SerializeField]
    GameObject IdleStatePanel;

    [SerializeField]
    GameObject UpgradeInProgressPanel;

    [SerializeField]
    GameObject UpgradeButton;

    [SerializeField]
    TextMeshProUGUI UpgradeLengthText;

    [SerializeField]
    TextMeshProUGUI UpgradePriceText;

    [SerializeField]
    Image UpgradeFillImage;

    [SerializeField]
    PortraitUI OwnerPortrait;

    [SerializeField]
    Transform EmployeeGrid;

    [SerializeField]
    Transform ActionGrid;

    [SerializeField]
    Image RecruitingPanel;

    LocationEntity CurrentLocation;

    public void Select(LocationEntity location)
    {
        CurrentLocation = location;
        CurrentLocation.StateUpdated.AddListener(RefreshUI);
        RefreshUI();
    }

    public void Deselect()
    {
        CurrentLocation.StateUpdated.RemoveListener(RefreshUI);
        CurrentLocation = null;
        Hide();
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        RefreshUI();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
        
    }

    public void RefreshUI()
    {
        RefreshPortraits();

        RefreshRanks();

        RefreshUpgradeState();

        RefreshActions();
    }

    void RefreshActions()
    {
        ClearActionInstances();

        ActionUI tempActionUI;
        for (int i = 0; i < CurrentLocation.CurrentProperty.Actions.Count; i++)
        {
            tempActionUI = ResourcesLoader.Instance.GetRecycledObject(ACTION_PREFAB).GetComponent<ActionUI>();
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
        OwnerPortrait.SetCharacter(CurrentLocation.OwnerCharacter);

        ClearEmployeeInstances();

        bool hasSetRecruitingBar = true;
        if(CurrentLocation.CurrentRecruitmentLength > 0)
        {
            RecruitingPanel.gameObject.SetActive(true);
            hasSetRecruitingBar = false;
        }
        else
        {
            RecruitingPanel.gameObject.SetActive(false);
        }

        PortraitUI tempPortrait;
        for (int i = 0; i < CurrentLocation.CurrentProperty.PropertyLevels[CurrentLocation.Level - 1].MaxEmployees; i++)
        {
            tempPortrait = ResourcesLoader.Instance.GetRecycledObject(PORTRAIT_PREFAB).GetComponent<PortraitUI>();
            tempPortrait.transform.SetParent(EmployeeGrid,false);
            tempPortrait.transform.localScale = Vector3.one;

            if (CurrentLocation.EmployeesCharacters.Count > i)
            {
                tempPortrait.SetCharacter(CurrentLocation.EmployeesCharacters[i]);
            }
            else
            {
                if (CurrentLocation.CurrentRecruitmentLength > 0 && !hasSetRecruitingBar)
                {
                    RecruitingPanel.gameObject.SetActive(true);

                    RecruitingPanel.fillAmount =
                        (float)CurrentLocation.CurrentRecruitmentLength
                        /
                        (float)CurrentLocation.CurrentProperty.PropertyLevels[CurrentLocation.Level].RecruitmentLength;

                    hasSetRecruitingBar = true;
                }

                tempPortrait.SetCharacter(null);
            }
        }
    }

    void ClearEmployeeInstances()
    {
        while (EmployeeGrid.childCount > 0)
        {
            EmployeeGrid.GetChild(0).gameObject.SetActive(false);
            EmployeeGrid.GetChild(0).SetParent(transform);
        }
    }

    void ClearActionInstances()
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
            IdleStatePanel.gameObject.SetActive(false);
            UpgradeButton.gameObject.SetActive(false);


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
            IdleStatePanel.gameObject.SetActive(true);

            if (CurrentLocation.CurrentProperty.PropertyLevels.Count > CurrentLocation.Level)
            {
                UpgradeButton.gameObject.SetActive(true);

                if (CurrentLocation.CurrentProperty.PropertyLevels.Count > CurrentLocation.Level)
                {
                    UpgradePriceText.text = CurrentLocation.CurrentProperty.PropertyLevels[CurrentLocation.Level].UpgradePrice.ToString();
                }
            }
            else
            {
                UpgradeButton.gameObject.SetActive(false);
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
}