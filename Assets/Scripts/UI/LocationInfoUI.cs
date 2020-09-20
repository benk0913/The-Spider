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
    Transform PeopleInLocationContainer;

    [SerializeField]
    Transform PeopleLivingContainer;

    [SerializeField]
    TextMeshProUGUI PropertyNameText;

    [SerializeField]
    TextMeshProUGUI RankText;

    [SerializeField]
    TextMeshProUGUI CorpsesBuriedLabel;

    [SerializeField]
    Transform TraitsContainer;

    [SerializeField]
    GameObject HasCaseInProgressPanel;

    LocationEntity CurrentLocation;


    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("HideMap", Hide);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
    }

    public void Hide()
    {
        CORE.Instance.UnsubscribeFromEvent("PassTimeComplete", RefreshUI);
        this.gameObject.SetActive(false);
    }

    public void Show(LocationEntity location)
    {
        CharacterInfoUI.Instance.Hide();
        FactionInfoUI.Instance.Hide();

        if (location == null)
        {
            return;
        }

        this.gameObject.SetActive(true);

        CurrentLocation = location;

        CORE.Instance.SubscribeToEvent("PassTimeComplete", RefreshUI);
        RefreshUI();
    }

    public void RefreshUI()
    {
        if(CurrentLocation == null)
        {
            return;
        }

        LocationPortrait.SetLocation(CurrentLocation);

        OwnerPortrait.SetCharacter(CurrentLocation.OwnerCharacter);

        PropertyNameText.text = CurrentLocation.Name;
        RankText.text = "Rank - " + CurrentLocation.Level;

        ClearContainer(EmployeesContainer);
        foreach (Character character in CurrentLocation.EmployeesCharacters)
        {
            GameObject tempPortrait = ResourcesLoader.Instance.GetRecycledObject("PortraitUI");
            tempPortrait.transform.SetParent(EmployeesContainer, false);
            tempPortrait.transform.localScale = Vector3.one;
            tempPortrait.GetComponent<PortraitUI>().SetCharacter(character);
        }

        ClearContainer(PeopleInLocationContainer);
        foreach (Character character in CurrentLocation.CharactersInLocation)
        {
            GameObject tempPortrait = ResourcesLoader.Instance.GetRecycledObject("PortraitUI");
            tempPortrait.transform.SetParent(PeopleInLocationContainer, false);
            tempPortrait.transform.localScale = Vector3.one;
            tempPortrait.GetComponent<PortraitUI>().SetCharacter(character);
        }

        ClearContainer(PeopleLivingContainer);
        foreach (Character character in CurrentLocation.CharactersLivingInLocation)
        {
            GameObject tempPortrait = ResourcesLoader.Instance.GetRecycledObject("PortraitUI");
            tempPortrait.transform.SetParent(PeopleLivingContainer, false);
            tempPortrait.transform.localScale = Vector3.one;
            tempPortrait.GetComponent<PortraitUI>().SetCharacter(character);
        }

        ClearTraits();
        for (int i = 0; i < CurrentLocation.Traits.Count; i++)
        {
            GameObject tempTrait = ResourcesLoader.Instance.GetRecycledObject("TraitUI");
            tempTrait.transform.SetParent(TraitsContainer, false);
            tempTrait.transform.localScale = Vector3.one;
            tempTrait.GetComponent<TraitUI>().SetInfo(CurrentLocation.Traits[i]);
        }

        HasCaseInProgressPanel.SetActive(CurrentLocation.CaseElements.Count > 0);

        CorpsesBuriedLabel.transform.parent.gameObject.SetActive(CurrentLocation.OwnerCharacter != null && CurrentLocation.OwnerCharacter.TopEmployer == CORE.PC && CurrentLocation.CorpsesBuried > 0);

        CorpsesBuriedLabel.text = CurrentLocation.CorpsesBuried.ToString();
    }

    void ClearContainer(Transform containerTransform)
    {
        while(containerTransform.childCount > 0)
        {
            containerTransform.GetChild(0).gameObject.SetActive(false);
            containerTransform.GetChild(0).SetParent(transform);
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

    public void SelectCurrentLocation()
    {
        SelectedPanelUI.Instance.Select(CurrentLocation);
    }
}
