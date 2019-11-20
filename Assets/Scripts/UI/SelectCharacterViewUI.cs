using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharacterViewUI : MonoBehaviour
{
    public static SelectCharacterViewUI Instance;

    [SerializeField]
    protected Transform Container;

    [SerializeField]
    public string PortraitPrefab = "SelectablePortraitUI";

    [SerializeField]
    public TextMeshProUGUI TitleText;
    
    protected virtual void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public virtual void Show(Action<Character> onSelect = null, Predicate<Character> filter = null, string title = "Select Agent:")
    {
        this.gameObject.SetActive(true);

        if(!this.gameObject.activeInHierarchy)
        {
            return;
        }

        if(PopulateGridRoutine != null)
        {
            return;
        }

        PopulateGridRoutine = StartCoroutine(PopulateGrid(onSelect, filter));
        TitleText.text = title;
    }

    protected Coroutine PopulateGridRoutine;
    protected virtual IEnumerator PopulateGrid(Action<Character> onSelect = null, Predicate<Character> filter = null)
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
            GameObject selectableChar = ResourcesLoader.Instance.GetRecycledObject(PortraitPrefab);

            selectableChar.transform.SetParent(Container, false);
            selectableChar.transform.localScale = Vector3.one;

            if (onSelect != null)
            {
                Button tempButton = selectableChar.GetComponent<Button>();
                tempButton.onClick.RemoveAllListeners();
                tempButton.onClick.AddListener(() =>
                {
                    onSelect.Invoke(character);
                    this.gameObject.SetActive(false);
                });
            }

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
