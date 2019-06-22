using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectedPanelUI : MonoBehaviour
{
    public static SelectedPanelUI Instance;

    public Character CurrentCharacter;

    [SerializeField]
    TextMeshProUGUI NameText;

    [SerializeField]
    PortraitUI Portrait;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void SetSelected(Character character)
    {
        if(!this.gameObject.activeInHierarchy)
            this.gameObject.SetActive(true);

        CurrentCharacter = character;
        RefreshUI();
    }

    public void Deselect()
    {
        this.gameObject.SetActive(false);
    }

    void RefreshUI()
    {
        NameText.text = CurrentCharacter.name;
        Portrait.SetCharacter(CurrentCharacter);
    }
}
