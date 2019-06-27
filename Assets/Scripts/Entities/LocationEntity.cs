using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocationEntity : MonoBehaviour
{
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
    TextMeshProUGUI UpgradeLengthText;

    [SerializeField]
    TextMeshProUGUI UpgradePriceText;

    [SerializeField]
    Image UpgradeFillImage;

    [SerializeField]
    Property CurrentProperty;

    [SerializeField]
    GameObject IdlePanel;

    [SerializeField]
    GameObject SelectedPanel;

    [SerializeField]
    Collider LocationColider;

    int Level = 1;

    public bool IsSelected;

    public bool IsUpgrading;

    public int CurrentUpgradeLength;

    public bool IsVisible = false;

    private void Start()
    {
        GameClock.Instance.OnTurnPassed.AddListener(TurnPassed);

        if(CurrentProperty != null)
        {
            RefreshUI();
        }
    }

    void TurnPassed()
    {
        if (IsUpgrading)
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

    public void OnClick()
    {
        MapViewManager.Instance.SelectLocation(this);
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
        //ClearEmployees(); - TODO Implement Later

        IdlePanel.gameObject.SetActive(true);
        SelectedPanel.gameObject.SetActive(false);
    }

    public void SetInfo(Property property)
    {
        CurrentProperty = property;

        LocationIcon.sprite = CurrentProperty.Icon;
        LocationTitle.text = CurrentProperty.name;

        //TODO Set prefab 
        RefreshUI();
    }

    public void RefreshUI()
    {
        for(int i=0;i< RanksContainer.childCount; i++)
        {
            RanksContainer.GetChild(i).gameObject.SetActive(Level > i);
        }

        if (IsUpgrading)
        {
            UpgradeInProgressPanel.SetActive(true);
            IdleStatePanel.gameObject.SetActive(false);
            

            GameClock.GameTimeLength upgradeLength = new GameClock.GameTimeLength(CurrentUpgradeLength);

            UpgradeLengthText.text = "<color=black>Ready In:</color>\n";
            UpgradeLengthText.text += ((GameClock.GameTime)upgradeLength.DayTime)+"\n";
            if (upgradeLength.Days > 0)
            {
                UpgradeLengthText.text += upgradeLength.Days.ToString()
                    + ((upgradeLength.Days == 1) ? " day from \n now..." : " days from \n now...");
            }
            
            UpgradeFillImage.fillAmount = ((float)CurrentUpgradeLength) / ((float)CurrentProperty.PropertyLevels[Level].UpgradeLength);
        }
        else
        {
            UpgradeInProgressPanel.SetActive(false);
            IdleStatePanel.gameObject.SetActive(true);

            UpgradePriceText.text = CurrentProperty.PropertyLevels[Level].UpgradePrice.ToString();
        }
    }


    public void PurchaseUpgrade()
    {
        if (Level >= CurrentProperty.PropertyLevels.Count)
        {
            return;
        }

        if (GameStorage.Instance.Gold < CurrentProperty.PropertyLevels[Level].UpgradePrice)
        {
            //TODO "NO MONEY ALERT"
            return;
        }

        GameStorage.Instance.Gold -= CurrentProperty.PropertyLevels[Level].UpgradePrice;
        IsUpgrading = true;
        CurrentUpgradeLength = CurrentProperty.PropertyLevels[Level].UpgradeLength;
        RefreshUI();
    }

    public void CancelUpgrade()
    {
        if (!IsUpgrading)
        {
            return;
        }

        GameStorage.Instance.Gold += CurrentProperty.PropertyLevels[Level].UpgradePrice;
        IsUpgrading = false;
        RefreshUI();
    }

}
