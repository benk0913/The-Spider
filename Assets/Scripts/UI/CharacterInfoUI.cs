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
    LocationPortraitUI HomeLocationPortrait;

    [SerializeField]
    Transform PropertiesOwnedContainer;

    [SerializeField]
    Transform TraitsContainer;

    [SerializeField]
    Transform BonusesContainer;

    [SerializeField]
    RelationUI RelationIcon;

    [SerializeField]
    ActionPortraitUI ActionPortrait;

    [SerializeField]
    Image PinImage;

    [SerializeField]
    Transform KnownInformationContainer;

    [SerializeField]
    GameObject DeceasedPanel;

    Character CurrentCharacter;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CORE.Instance.SubscribeToEvent("HideMap",Hide);
        CORE.Instance.SubscribeToEvent("PassTimeComplete", RefreshTurnPassed);
        this.gameObject.SetActive(false);
    }

    void RefreshTurnPassed()
    {
        if(CurrentCharacter == null)
        {
            return;
        }

        SetGold();
        SetCurrentLocation();
    }


    void SetName()
    {
        if (CurrentCharacter.IsKnown("Name", CORE.PC))
        {
            NameText.text = CurrentCharacter.name;
        }
        else
        {
            NameText.text = "???";
        }
    }

    void SetGold()
    {
        if (CurrentCharacter.IsKnown("Gold", CORE.PC))
        {
            GoldText.text = CurrentCharacter.Gold + "c";
        }
        else
        {
            GoldText.text = "???";
        }
    }

    void SetAppearance()
    {
        Portrait.SetCharacter(CurrentCharacter);

        if (CurrentCharacter.IsKnown("Appearance", CORE.PC))
        {
            AgeText.text = "Age: " + CurrentCharacter.Age.ToString();
            AgeTypeText.text = CurrentCharacter.AgeType.ToString();
            GenderText.text = CurrentCharacter.Gender.ToString();
        }
        else
        {
            AgeText.text = "Age: ???";
            AgeTypeText.text = "???";
            GenderText.text = "???";
        }
    }

    void SetPersonality()
    {
        ClearTraits();

        if (CurrentCharacter.IsKnown("Personality", CORE.PC))
        {
            for (int i = 0; i < CurrentCharacter.Traits.Count; i++)
            {
                GameObject tempTrait = ResourcesLoader.Instance.GetRecycledObject("TraitUI");
                tempTrait.transform.SetParent(TraitsContainer, false);
                tempTrait.transform.localScale = Vector3.one;

                tempTrait.GetComponent<TraitUI>().SetInfo(CurrentCharacter.Traits[i]);
            }
        }
        else
        {
            for (int i = 0; i < CurrentCharacter.Traits.Count; i++)
            {
                GameObject tempTrait = ResourcesLoader.Instance.GetRecycledObject("TraitUI");
                tempTrait.transform.SetParent(TraitsContainer, false);
                tempTrait.transform.localScale = Vector3.one;

                tempTrait.GetComponent<TraitUI>().SetInfo(CORE.Instance.Database.UnknownTrait);
            }
        }

    }

    void SetCurrentLocation()
    {
        if (CurrentCharacter.IsKnown("CurrentLocation", CORE.PC))
        {
            if (CurrentCharacter.CurrentLocation != null)
            {
                CurrentLocationText.text = "Location: " + CurrentCharacter.CurrentLocation.CurrentProperty.name;
            }

            if (CurrentCharacter.CurrentTaskEntity != null)
            {
                ActionPortrait.gameObject.SetActive(true);
                ActionPortrait.SetAction(CurrentCharacter.CurrentTaskEntity);
            }
            else
            {
                ActionPortrait.gameObject.SetActive(false);
            }
        }
        else
        {
            CurrentLocationText.text = "Location: ???";
            ActionPortrait.gameObject.SetActive(false);
        }
    }

    void SetWorkLocation()
    {
        ClearPropertiesOwned();

        if (CurrentCharacter.IsKnown("WorkLocation", CORE.PC))
        {
            EmployerPortrait.SetCharacter(CurrentCharacter.Employer);
            WorkLocationPortrait.SetLocation(CurrentCharacter.WorkLocation);

            for (int i = 0; i < CurrentCharacter.PropertiesOwned.Count; i++)
            {
                GameObject tempPortrait = ResourcesLoader.Instance.GetRecycledObject("LocationPortraitUI");
                tempPortrait.transform.SetParent(PropertiesOwnedContainer, false);
                tempPortrait.transform.localScale = Vector3.one;
                tempPortrait.GetComponent<LocationPortraitUI>().SetLocation(CurrentCharacter.PropertiesOwned[i]);
            }
        }
        else
        {
            EmployerPortrait.SetCharacter(null);
            WorkLocationPortrait.SetLocation(null);

            for (int i = 0; i < CurrentCharacter.PropertiesOwned.Count; i++)
            {
                GameObject tempPortrait = ResourcesLoader.Instance.GetRecycledObject("LocationPortraitUI");
                tempPortrait.transform.SetParent(PropertiesOwnedContainer, false);
                tempPortrait.transform.localScale = Vector3.one;
                tempPortrait.GetComponent<LocationPortraitUI>().SetLocation(null);
            }
        }
    }

    void SetHomeLocation()
    {
        if (CurrentCharacter.IsKnown("HomeLocation", CORE.PC))
        {
            HomeLocationPortrait.SetLocation(CurrentCharacter.HomeLocation);
        }
        else
        {
            HomeLocationPortrait.SetLocation(null);
        }
    }

    void SetSkills()
    {
        ClearBonuses();

        if (CurrentCharacter.IsKnown("Personality", CORE.PC))
        {
            for (int i = 0; i < CurrentCharacter.Bonuses.Count; i++)
            {
                if (CurrentCharacter.Bonuses[i].Value < 2)
                {
                    continue;
                }

                GameObject tempBonus = ResourcesLoader.Instance.GetRecycledObject("BonusUI");
                tempBonus.transform.SetParent(BonusesContainer, false);
                tempBonus.transform.localScale = Vector3.one;
                tempBonus.GetComponent<BonusUI>().SetInfo(CurrentCharacter.Bonuses[i]);
            }
        }
        else
        {
            for (int i = 0; i < CurrentCharacter.Bonuses.Count; i++)
            {
                if (CurrentCharacter.Bonuses[i].Value < 2)
                {
                    continue;
                }

                GameObject tempBonus = ResourcesLoader.Instance.GetRecycledObject("BonusUI");
                tempBonus.transform.SetParent(BonusesContainer, false);
                tempBonus.transform.localScale = Vector3.one;
                tempBonus.GetComponent<BonusUI>().SetInfo(null);
            }
        }
    }

    void SetRelation()
    {
        if (CurrentCharacter.IsKnown("Personality", CORE.PC))
        {
            RelationIcon.SetInfo(CurrentCharacter, CORE.PC);
        }
        else
        {
            RelationIcon.SetInfo(null, null);
        }
    }

    public void ShowInfo(Character character)
    {
        LocationInfoUI.Instance.Hide();

        if (character == null)
        {
            return;
        }

        CurrentCharacter = character;

        this.gameObject.SetActive(true);

        PinImage.color = character.Pinned ? Color.yellow : Color.black;
        ControlButton.interactable = (character.TopEmployer != character && character.TopEmployer == CORE.PC);
        XImage.SetActive(!ControlButton.interactable);

        //Known Info
        ClearKnownInfo();

        foreach (KnowledgeInstance kInstance in CurrentCharacter.Known.Items)
        {
            KnownInstanceUI uiInstance = ResourcesLoader.Instance.GetRecycledObject("KnownInstanceUI").GetComponent<KnownInstanceUI>();
            uiInstance.SetInfo(kInstance.Key, kInstance.Description, kInstance.IsKnownByCharacter(CORE.PC));
            uiInstance.transform.SetParent(KnownInformationContainer, false);
        }

        DeceasedPanel.gameObject.SetActive(character.IsDead);

        SetName();
        SetGold();
        SetAppearance();
        SetPersonality();
        SetCurrentLocation();
        SetWorkLocation();
        SetHomeLocation();
        SetSkills();
        SetRelation();
    }

    void ClearPropertiesOwned()
    {
        while(PropertiesOwnedContainer.childCount > 0)
        {
            PropertiesOwnedContainer.GetChild(0).gameObject.SetActive(false);
            PropertiesOwnedContainer.GetChild(0).SetParent(transform);
        }
    }

    void ClearBonuses()
    {
        while (BonusesContainer.childCount > 0)
        {
            BonusesContainer.GetChild(0).gameObject.SetActive(false);
            BonusesContainer.GetChild(0).SetParent(transform);
        }
    }

    void ClearKnownInfo()
    {
        while (KnownInformationContainer.childCount > 0)
        {
            KnownInformationContainer.GetChild(0).gameObject.SetActive(false);
            KnownInformationContainer.GetChild(0).SetParent(transform);
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

    public void TogglePin()
    {
        CurrentCharacter.Pinned = !CurrentCharacter.Pinned;
        PinImage.color = CurrentCharacter.Pinned ? Color.yellow : Color.black;
    }
}
