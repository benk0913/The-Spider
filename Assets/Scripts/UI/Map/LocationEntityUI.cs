using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocationEntityUI : MonoBehaviour
{
    #region SerializedFields
    [SerializeField]
    GameObject IdlePanel;

    [SerializeField]
    GameObject SelectedPanel;

    [SerializeField]
    Property PropertyReference;

    [SerializeField]
    Image PropertyIcon;

    [SerializeField]
    TextMeshProUGUI TitleTextHover;
    [SerializeField]
    TextMeshProUGUI TitleTextSelected;
    [SerializeField]
    TextMeshProUGUI UpgradePriceText;
    [SerializeField]
    TextMeshProUGUI CurrentLevelText;
    [SerializeField]
    TextMeshProUGUI UpgradeLengthDaytimeText;
    [SerializeField]
    TextMeshProUGUI UpgradeLengthDaysCountText;

    [SerializeField]
    Image UpgradeFillImage;

    [SerializeField]
    Image UpgradePanel;

    [SerializeField]
    GameObject UpgradeInProgressPanel;

    [SerializeField]
    Transform EmployeesGrid;

    [SerializeField]
    PortraitUI OwnerPortrait;

    #endregion

    #region Stats

    public int Level = 1;

    public Character Owner;

    public List<Character> Employees = new List<Character>();

    public bool IsSelected = false;

    public bool IsUpgrading;

    public int CurrentUpgradeLength;

    #endregion

    private void Start()
    {
        if (PropertyReference != null)
            RefreshUI();

        GameClock.Instance.OnTurnPassed.AddListener(TurnPassed);
    }

    void TurnPassed()
    {
        if(IsUpgrading)
        {
            CurrentUpgradeLength--;
            
            if (CurrentUpgradeLength <= 0)
            {
                IsUpgrading = false;
                Level++;
            }

            RefreshUI();
        }
    }

    void RefreshUI()
    {
        PropertyIcon.sprite = PropertyReference.Icon;
        TitleTextHover.text = PropertyReference.name;
        TitleTextSelected.text = PropertyReference.name;
        CurrentLevelText.text = Level.ToString();

        if (PropertyReference.PropertyLevels.Count <= Level || IsUpgrading)
        {
            UpgradePanel.gameObject.SetActive(false);
        }
        else
        {
            UpgradePanel.gameObject.SetActive(true);
            UpgradePanel.color =
                (GameStorage.Instance.Gold >= PropertyReference.PropertyLevels[Level].UpgradePrice) ?
                Color.green : Color.red;

            UpgradePriceText.text = PropertyReference.PropertyLevels[Level].UpgradePrice+"c";
        }

        if(IsSelected)
        {
            IdlePanel.gameObject.SetActive(false);
            SelectedPanel.gameObject.SetActive(true);

            OwnerPortrait.SetCharacter(Owner);
            if (Owner == null)
            {
                OwnerPortrait.SetDisabled();
            }

            ClearEmployees();

            GameObject tempPort;
            for (int i = 0; i < PropertyReference.PropertyLevels[Level - 1].MaxEmployees; i++)
            {
                tempPort = ResourcesLoader.Instance.GetRecycledObject("PortraitUI");
                tempPort.transform.SetParent(EmployeesGrid.transform, false);
                tempPort.transform.localScale = Vector3.one;

                if (Employees.Count > i)
                {
                    tempPort.GetComponent<PortraitUI>().SetCharacter(Employees[i]);
                }
                else
                {
                    tempPort.GetComponent<PortraitUI>().SetCharacter(null);
                }
            }

            if(IsUpgrading)
            {
                if (!UpgradeInProgressPanel.activeInHierarchy)
                    UpgradeInProgressPanel.SetActive(true);

                GameClock.GameTimeLength upgradeLength = new GameClock.GameTimeLength(CurrentUpgradeLength);

                if (upgradeLength.Days > 0)
                {
                    UpgradeLengthDaysCountText.text = upgradeLength.Days.ToString()
                        + ((upgradeLength.Days == 1) ?  " day from now." : " days from now.");
                }
                else
                {
                    UpgradeLengthDaysCountText.text = "";
                }

                UpgradeLengthDaytimeText.text = ((GameClock.GameTime) upgradeLength.DayTime).ToString();

                UpgradeFillImage.fillAmount = ((float)CurrentUpgradeLength) / ((float)PropertyReference.PropertyLevels[Level].UpgradeLength);
            }
            else
            {
                if(UpgradeInProgressPanel.activeInHierarchy)
                    UpgradeInProgressPanel.SetActive(false);
                
            }
        }
    }

    public void OnClick()
    {
       // MapViewManager.Instance.SelectLocation(this);
    }

    public void Select()
    {
        IsSelected = true;
        RefreshUI();

        IdlePanel.gameObject.SetActive(false);
        SelectedPanel.gameObject.SetActive(true);
    }

    public void Deselect()
    {
        IsSelected = false;
        ClearEmployees();

        IdlePanel.gameObject.SetActive(true);
        SelectedPanel.gameObject.SetActive(false);
    }

    void ClearEmployees()
    {
        while (EmployeesGrid.childCount > 0)
        {
            EmployeesGrid.GetChild(0).gameObject.SetActive(false);
            EmployeesGrid.GetChild(0).SetParent(transform);
        }
    }

    public void PurchaseUpgrade()
    {
        if(Level >= PropertyReference.PropertyLevels.Count)
        {
            return;
        }

        if(GameStorage.Instance.Gold < PropertyReference.PropertyLevels[Level].UpgradePrice)
        {
            //TODO "NO MONEY ALERT"
            return;
        }

        GameStorage.Instance.Gold -= PropertyReference.PropertyLevels[Level].UpgradePrice;
        IsUpgrading = true;
        CurrentUpgradeLength = PropertyReference.PropertyLevels[Level + 1].UpgradeLength;
        RefreshUI();
    }

    public void CancelUpgrade()
    {
        if(!IsUpgrading)
        {
            return;
        }

        GameStorage.Instance.Gold += PropertyReference.PropertyLevels[Level + 1].UpgradePrice;
        IsUpgrading = false;
        RefreshUI();
    }
}
