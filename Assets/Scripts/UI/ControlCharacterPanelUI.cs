using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControlCharacterPanelUI : MonoBehaviour
{
    [SerializeField]
    PortraitUI Portrait;

    [SerializeField]
    TextMeshProUGUI NameText;

    public static Character CurrentCharacter;

    public void SetInfo(Character character)
    {
        CurrentCharacter = character;
        RefreshUI();
    }

    void RefreshUI()
    {
        NameText.text = CurrentCharacter.name;
        Portrait.SetCharacter(CurrentCharacter);
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

    public void ShowInfo()
    {
        CharacterInfoUI.Instance.ShowInfo(CurrentCharacter);
    }

    
}
