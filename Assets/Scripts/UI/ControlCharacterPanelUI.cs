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

    CommandChainModule CurrentChainModule;

    public void Select(Character character)
    {
        Deselect();

        CurrentCharacter = character;
        RefreshUI();
    }

    public void Deselect()
    {
        if(CurrentCharacter == null)
        {
            return;
        }

        CurrentCharacter = null;

        ClearCurrentChain();
    }

    void RefreshUI()
    {
        NameText.text = CurrentCharacter.name;
        Portrait.SetCharacter(CurrentCharacter);

        ShowCommandChain();
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        Deselect();
        this.gameObject.SetActive(false);
    }

    public void ShowInfo()
    {
        CharacterInfoUI.Instance.ShowInfo(CurrentCharacter);
    }

    void ShowCommandChain()
    {
        if(CurrentCharacter == null)
        {
            return;
        }

        if(CurrentChainModule != null)
        {
            ClearCurrentChain();
        }

        CurrentChainModule = ResourcesLoader.Instance.GetRecycledObject("CommandChain").GetComponent<CommandChainModule>();
        CurrentChainModule.SetInfo(CurrentCharacter);
    }

    void ClearCurrentChain()
    {
        if (CurrentChainModule != null)
        {
            CurrentChainModule.Hide();
            CurrentChainModule.gameObject.SetActive(false);
            CurrentChainModule = null;
        }
    }

}
