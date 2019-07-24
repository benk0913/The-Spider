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
    Button ControlButton;

    [SerializeField]
    GameObject XImage;

    [SerializeField]
    PortraitUI Portrait;

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

        ControlButton.interactable = (character.TopEmployer != character && character.TopEmployer == CORE.PC);
        XImage.SetActive(!ControlButton.interactable);

        Portrait.SetCharacter(character);
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
