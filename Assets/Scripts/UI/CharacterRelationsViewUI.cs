using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterRelationsViewUI : SelectCharacterViewUI
{
    public static CharacterRelationsViewUI Instance;

    public Character CurrentCharacter;

    [SerializeField]
    protected PortraitUI CurrentCharacterPortrait;

    protected override void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Show(Character relevantCharacter, Action<Character> onSelect = null, Predicate<Character> filter = null, string title = "Select Agent:")
    {
        CurrentCharacter = relevantCharacter;
        CurrentCharacterPortrait.SetCharacter(CurrentCharacter);

        base.Show(onSelect, filter, title);
    }

    protected override IEnumerator PopulateGrid(Action<Character> onSelect = null, Predicate<Character> filter = null)
    {
        while (Container.childCount > 0)
        {
            Container.GetChild(0).gameObject.SetActive(false);
            Container.GetChild(0).SetParent(transform);
        }

        yield return 0;

        List<Character> characters = CORE.Instance.Characters.FindAll(filter != null ? filter : CommonFilter);

        yield return 0;

        foreach (Character character in characters)
        {
            GameObject selectableChar = ResourcesLoader.Instance.GetRecycledObject(PortraitPrefab);

            selectableChar.transform.SetParent(Container, false);
            selectableChar.transform.localScale = Vector3.one;

            selectableChar.transform.GetComponentInChildren<PortraitUI>().SetCharacter(character);
            selectableChar.transform.GetComponentInChildren<RelationUI>().SetInfo(CurrentCharacter, character);
            yield return 0;
        }

        PopulateGridRoutine = null;
    }

    public override bool CommonFilter(Character character)
    {
        return character.Known.GetIsAnythingKnown(CORE.PC) && character != CurrentCharacter;
    }

}
