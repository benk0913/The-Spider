using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoUI : MonoBehaviour
{
    public static CharacterInfoUI Instance;

    [SerializeField]
    TextMeshProUGUI NameText;

    [SerializeField]
    TextMeshProUGUI GoldText;

    [SerializeField]
    TextMeshProUGUI AgeText;

    [SerializeField]
    TextMeshProUGUI AgeTypeText;

    [SerializeField]
    TextMeshProUGUI GenderText;

    [SerializeField]
    TextMeshProUGUI CurrentLocationText;

    [SerializeField]
    Button ControlButton;

    [SerializeField]
    GameObject XImage;

    [SerializeField]
    PortraitUI Portrait;

    [SerializeField]
    PortraitUI EmployerPortrait;

    [SerializeField]
    LocationPortraitUI WorkLocationPortrait;

    [SerializeField]
    Transform PropertiesOwnedContainer;

    [SerializeField]
    Transform TraitsContainer;

    Character CurrentCharacter;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CORE.Instance.SubscribeToEvent("HideMap",Hide);
        this.gameObject.SetActive(false);
    }

    public void ShowInfo(Character character)
    {
        if(character == null)
        {
            return;
        }

        CurrentCharacter = character;

        this.gameObject.SetActive(true);

        NameText.text    = character.name;
        GoldText.text = character.Gold + "c";
        AgeText.text     = "Age: "+character.Age.ToString();
        AgeTypeText.text = character.AgeType.ToString();
        GenderText.text  = character.Gender.ToString();
        CurrentLocationText.text = "Location: " + character.CurrentLocation.CurrentProperty.name;

        ControlButton.interactable = (character.TopEmployer != character && character.TopEmployer == CORE.PC);
        XImage.SetActive(!ControlButton.interactable);

        Portrait.SetCharacter(character);
        EmployerPortrait.SetCharacter(character.Employer);
        WorkLocationPortrait.SetLocation(character.WorkLocation);

        ClearPropertiesOwned();
        for(int i=0;i<CurrentCharacter.PropertiesOwned.Count;i++)
        {
            GameObject tempPortrait = ResourcesLoader.Instance.GetRecycledObject("LocationPortraitUI");
            tempPortrait.transform.SetParent(PropertiesOwnedContainer, false);
            tempPortrait.transform.localScale = Vector3.one;
            tempPortrait.GetComponent<LocationPortraitUI>().SetLocation(CurrentCharacter.PropertiesOwned[i]);
        }


        ClearTraits();
        for (int i = 0; i < CurrentCharacter.Traits.Count; i++)
        {
            GameObject tempTrait = ResourcesLoader.Instance.GetRecycledObject("TraitUI");
            tempTrait.transform.SetParent(TraitsContainer, false);
            tempTrait.transform.localScale = Vector3.one;
            tempTrait.GetComponent<TraitUI>().SetInfo(CurrentCharacter.Traits[i]);
        }
    }

    void ClearPropertiesOwned()
    {
        while(PropertiesOwnedContainer.childCount > 0)
        {
            PropertiesOwnedContainer.GetChild(0).gameObject.SetActive(false);
            PropertiesOwnedContainer.GetChild(0).SetParent(transform);
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

    public void Command()
    {
        if (this.CurrentCharacter == null)
        {
            return;
        }

        if (CurrentCharacter.Employer == null)
        {
            return;
        }

        if (CurrentCharacter.TopEmployer != CORE.PC)
        {
            return;
        }

        Hide();
        SelectedPanelUI.Instance.SetSelected(this.CurrentCharacter);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
