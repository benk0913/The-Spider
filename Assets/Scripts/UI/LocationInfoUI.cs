using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocationInfoUI : MonoBehaviour
{
    public static LocationInfoUI Instance;

    [SerializeField]
    LocationPortraitUI LocationPortrait;

    [SerializeField]
    PortraitUI OwnerPortrait;

    [SerializeField]
    Transform EmployeesContainer;

    [SerializeField]
    TextMeshProUGUI PropertyNameText;

    [SerializeField]
    TextMeshProUGUI RankText;

    [SerializeField]
    TextMeshProUGUI RevenueMultiText;

    [SerializeField]
    TextMeshProUGUI RiskMultiText;

    [SerializeField]
    Transform TraitsContainer;

    LocationEntity CurrentLocation;


    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Show(LocationEntity location)
    {
        CharacterInfoUI.Instance.Hide();

        if (location == null)
        {
            return;
        }

        this.gameObject.SetActive(true);

        CurrentLocation = location;


        LocationPortrait.SetLocation(location);

        OwnerPortrait.SetCharacter(location.OwnerCharacter);

        PropertyNameText.text = location.CurrentProperty.name;
        RankText.text = "Rank - " + location.Level;
        RevenueMultiText.text = "Revenue Multiplier - x" + location.RevneueMultiplier;
        RiskMultiText.text = "Risk Multiplier - x" + location.RiskMultiplier;
        
        ClearEmployeesContainer();
        foreach(Character character in location.EmployeesCharacters)
        {
            GameObject tempPortrait = ResourcesLoader.Instance.GetRecycledObject("PortraitUI");
            tempPortrait.transform.SetParent(EmployeesContainer, false);
            tempPortrait.transform.localScale = Vector3.one;
            tempPortrait.GetComponent<PortraitUI>().SetCharacter(character);
        }

        ClearTraits();
        for (int i = 0; i < CurrentLocation.CurrentProperty.Traits.Count; i++)
        {
            GameObject tempTrait = ResourcesLoader.Instance.GetRecycledObject("TraitUI");
            tempTrait.transform.SetParent(TraitsContainer, false);
            tempTrait.transform.localScale = Vector3.one;
            tempTrait.GetComponent<TraitUI>().SetInfo(CurrentLocation.CurrentProperty.Traits[i]);
        }
    }

    void ClearEmployeesContainer()
    {
        while(EmployeesContainer.childCount > 0)
        {
            EmployeesContainer.GetChild(0).gameObject.SetActive(false);
            EmployeesContainer.GetChild(0).SetParent(transform);
        }
    }

    void ClearTraits()
    {
        while (TraitsContainer.childCount > 0)
        {
            TraitsContainer.GetChild(0).gameObject.SetActive(false);
            TraitsContainer.GetChild(0).SetParent(transform);
        }
    }

}
