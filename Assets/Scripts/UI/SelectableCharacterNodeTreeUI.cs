using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class SelectableCharacterNodeTreeUI : CharacterNodeTreeUI
{
    Action<Character> LocalOnSelect;

    public UnityEvent OnSelected;

    public void SetSelectableCharacters(Character topCharacter, Action<Character> onSelect = null)
    {
        LocalOnSelect = onSelect;
        ShowCharactersHirarchy(topCharacter);
    }

    protected override IEnumerator SetCharacters(CharacterNodeTreeUIInstance node)
    {
        node.nodeObject.transform.GetChild(0).GetChild(0).GetComponent<PortraitUI>().SetCharacter(node.CurrentCharacter);

        if (LocalOnSelect != null)
        {
            Button tempButton = node.nodeObject.transform.GetChild(0).GetChild(0).GetComponent<Button>();
            tempButton.onClick.RemoveAllListeners();
            tempButton.onClick.AddListener(() =>
            {
                if (node.CurrentCharacter != CORE.PC)
                {
                    LocalOnSelect.Invoke(node.CurrentCharacter);
                    OnSelected.Invoke();
                }
            });
        }

        yield return 0;

        for (int i = 0; i < node.Children.Count; i++)
        {
            yield return StartCoroutine(SetCharacters((CharacterNodeTreeUIInstance) node.Children[i]));
        }
    }
}

