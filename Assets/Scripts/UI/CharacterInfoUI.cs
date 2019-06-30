using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterInfoUI : MonoBehaviour
{
    public static CharacterInfoUI Instance;

    [SerializeField]
    TextMeshProUGUI NameText;

    [SerializeField]
    PortraitUI Portrait;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowInfo(Character character)
    {
        this.gameObject.SetActive(true);
        NameText.text = character.name;
        Portrait.SetCharacter(character);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
