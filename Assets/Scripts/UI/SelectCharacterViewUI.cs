using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharacterViewUI : MonoBehaviour
{
    [SerializeField]
    protected Transform Container;
    
    public virtual void Show(Action<Character> onSelect, Predicate<Character> filter = null)
    {
        if(PopulateGridRoutine != null)
        {
            return;
        }

        PopulateGridRoutine = StartCoroutine(PopulateGrid(onSelect, filter));
    }

    protected Coroutine PopulateGridRoutine;
    protected IEnumerator PopulateGrid(Action<Character> onSelect, Predicate<Character> filter = null)
    {
        while(Container.childCount > 0)
        {
            Container.GetChild(0).gameObject.SetActive(false);
            Container.GetChild(0).SetParent(transform);
        }

        yield return 0;

        List<Character> characters = CORE.Instance.Characters.FindAll(filter != null? filter : CommonFilter);

        yield return 0;

        foreach (Character character in characters)
        {
            GameObject selectableChar = ResourcesLoader.Instance.GetRecycledObject("SelectablePortraitUI");

            selectableChar.transform.SetParent(Container, false);

            Button tempButton = selectableChar.GetComponent<Button>();
            tempButton.onClick.RemoveAllListeners();
            tempButton.onClick.AddListener(() => 
            {
                onSelect.Invoke(character);
                this.gameObject.SetActive(false);
            });

            selectableChar.transform.GetComponentInChildren<PortraitUI>().SetCharacter(character);
            yield return 0;
        }

        PopulateGridRoutine = null;
    }

    public virtual bool CommonFilter(Character character)
    {
        return true;
    }
}
