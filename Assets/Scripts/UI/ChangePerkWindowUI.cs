using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using System;

public class ChangePerkWindowUI : MonoBehaviour
{
    public static ChangePerkWindowUI Instance;

    [SerializeField]
    TextMeshProUGUI RemovePerkTitle;

    [SerializeField]
    TextMeshProUGUI AddPerkTitle;

    [SerializeField]
    Transform PerkContainer;

    [SerializeField]
    PortraitUI CharacterPortrait;

    Character CurrentCharacter;

    public bool IsRemove;

    public int MaxSelection = 3;
    
    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (MouseLook.Instance == null) return;

        MouseLook.Instance.CurrentWindow = null;
    }


    public void Show(Character character, bool isRemove = false)
    {
        MouseLook.Instance.CurrentWindow = this.gameObject;

        this.gameObject.SetActive(true);
        CurrentCharacter = character;
        this.IsRemove = isRemove;


        CharacterPortrait.SetCharacter(CurrentCharacter);

        RemovePerkTitle.gameObject.SetActive(IsRemove);
        AddPerkTitle.gameObject.SetActive(!IsRemove);

        ClearContainer();

        List<Trait> traits = new List<Trait>();
        if(isRemove)
        {
            traits.AddRange(CurrentCharacter.Traits);
            traits.RemoveAll(x => !x.Manipulable);
        }
        else
        {
            traits.AddRange(CORE.Instance.Database.Traits);
            traits.RemoveAll(x => !x.Manipulable && CurrentCharacter.Traits.Find(y => y.name == x.name) != null);
        }

        traits = traits.OrderBy(x => Guid.NewGuid()).ToList();

        for(int i=0;i<MaxSelection;i++)
        {
            if(i>=traits.Count)
            {
                break;
            }

            GameObject traitObj = ResourcesLoader.Instance.GetRecycledObject("ClickableTrait");
            traitObj.transform.SetParent(PerkContainer, false);
            traitObj.GetComponent<ClickableTraitUI>().SetInfo(traits[i]);
        }
        
    }

    public void SelectTrait(Trait trait)
    {
        if (IsRemove)
        {
            CurrentCharacter.RemoveTrait(trait);
        }
        else
        {
            CurrentCharacter.AddTrait(trait);
        }

        Hide();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
        CharacterInfoUI.Instance.ShowInfo(CurrentCharacter);
    }

    void ClearContainer()
    {
        while(PerkContainer.childCount > 0)
        {
            PerkContainer.GetChild(0).gameObject.SetActive(false);
            PerkContainer.GetChild(0).SetParent(transform);
        }
    }

}
