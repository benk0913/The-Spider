﻿using System.Collections;
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
    Image RecruitingPanel;

    [SerializeField]
    GameObject RebrandBlockedSymbol;

    [SerializeField]
    LocationPortraitUI LocationPortrait;

    LocationEntity CurrentLocation;

    public void Select(LocationEntity location)
    {
        Deselect();

        CurrentLocation = location;
        CurrentLocation.SetSelected();
        CurrentLocation.StateUpdated.AddListener(RefreshUI);
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
        GameClock.Instance.OnTurnPassed.AddListener(OnTurnPassed);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
        GameClock.Instance.OnTurnPassed.RemoveListener(OnTurnPassed);
        Deselect();
    }

    void OnTurnPassed()
    {
        RefreshPortraits();
    }

    public void RefreshUI()
    {
        RebrandBlockedSymbol.gameObject.SetActive(CurrentLocation.CurrentProperty.PlotType == CORE.Instance.Database.UniquePlotType);
        LocationTitle.text = CurrentLocation.CurrentProperty.name;
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
    }

    void RefreshActions()
    {
        ClearActionInstances();

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

        OwnerPortrait.SetCharacter(CurrentLocation.OwnerCharacter, string.Format("x{0:F1}", productivity), productivity >= 1f);

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

        PortraitUIEmployee tempPortrait;
        for (int i = 0; i < CurrentLocation.CurrentProperty.PropertyLevels[CurrentLocation.Level - 1].MaxEmployees; i++)
        {
            tempPortrait = ResourcesLoader.Instance.GetRecycledObject("PortraitUIEmployee").GetComponent<PortraitUIEmployee>();
            tempPortrait.transform.SetParent(EmployeeGrid,false);
            tempPortrait.transform.localScale = Vector3.one;

            if (CurrentLocation.EmployeesCharacters.Count > i)
            {
                productivity = 1f;

                foreach (BonusChallenge bonusChallenge in CurrentLocation.CurrentAction.ActionBonusChallenges)
                {
                    productivity = CurrentLocation.EmployeesCharacters[i].GetBonus(bonusChallenge.Type).Value / bonusChallenge.ChallengeValue;
                }

                tempPortrait.SetCharacter(CurrentLocation.EmployeesCharacters[i], string.Format("x{0:F1}", productivity), productivity >= 1f); 
            }
            else
            {
                if (CurrentLocation.CurrentRecruitmentLength > 0 && !hasSetRecruitingBar)
                {
                    RecruitingPanel.gameObject.SetActive(true);

                    RecruitingPanel.fillAmount =
                        (float)CurrentLocation.CurrentRecruitmentLength
                        /
                        (float)CurrentLocation.CurrentProperty.PropertyLevels[CurrentLocation.Level-1].RecruitmentLength;

                    hasSetRecruitingBar = true;
                }

                tempPortrait.SetCharacter(null, "--");
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
}
