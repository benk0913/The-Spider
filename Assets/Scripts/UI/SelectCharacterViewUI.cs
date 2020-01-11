using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    Action<Character> CurrentonSelect;

    Predicate<Character> CurrentFilter;


    string CurrentTitle;

    public string CurrentSortKey;
    
    protected virtual void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (MouseLook.Instance == null) return;

        MouseLook.Instance.CurrentWindow = null;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            this.gameObject.SetActive(false);
        }
    }

    public virtual void Show(Action<Character> onSelect = null, Predicate<Character> filter = null, string title = "Select Agent:", Character topCharacter = null)
    {
        MouseLook.Instance.CurrentWindow = this.gameObject;

        this.gameObject.SetActive(true);

        CurrentTitle = title;
        CurrentonSelect = onSelect;
        CurrentFilter = filter;

        Refresh();
    }

    void Refresh()
    {
        if (!this.gameObject.activeInHierarchy)
        {
            return;
        }

        if (PopulateGridRoutine != null)
        {
            return;
        }

        PopulateGridRoutine = StartCoroutine(PopulateGrid(CurrentonSelect, CurrentFilter));
        TitleText.text = CurrentTitle;
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

        characters = SortCharacters(CurrentSortKey, characters);

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

    public void SetSortingKey(string sortingKey)
    {
        CurrentSortKey = sortingKey;
        Refresh();
    }

    public List<Character> SortCharacters(string byKey, List<Character> characters)
    {

        switch(byKey)
        {
            case "Skill_Strong":
                {
                    return characters.OrderByDescending(x => x.GetBonus(CORE.Instance.Database.GetBonusType("Strong")).Value).ToList();
                }
            case "Skill_Charming":
                {
                    return characters.OrderByDescending(x => x.GetBonus(CORE.Instance.Database.GetBonusType("Charming")).Value).ToList();
                }
            case "Skill_Intelligent":
                {
                    return characters.OrderByDescending(x => x.GetBonus(CORE.Instance.Database.GetBonusType("Intelligent")).Value).ToList();
                }
            case "Skill_Aware":
                {
                    return characters.OrderByDescending(x => x.GetBonus(CORE.Instance.Database.GetBonusType("Aware")).Value).ToList();
                }
            case "Skill_Discreet":
                {
                    return characters.OrderByDescending(x => x.GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value).ToList();
                }
            case "Skill_Menacing":
                {
                    return characters.OrderByDescending(x => x.GetBonus(CORE.Instance.Database.GetBonusType("Menacing")).Value).ToList();
                }
            case "Skill_Stealthy":
                {
                    return characters.OrderByDescending(x => x.GetBonus(CORE.Instance.Database.GetBonusType("Menacing")).Value).ToList();
                }
            case "Age":
                {
                    return characters.OrderByDescending(x => x.Age).ToList();
                }
            case "Rank":
                {
                    return characters.OrderBy(x => x.Rank).ToList();
                }
        }

        //Default Abc...
        return characters.OrderBy(x => x.name).ToList();
    }
}
