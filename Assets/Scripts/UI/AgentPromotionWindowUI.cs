using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AgentPromotionWindowUI : SelectCharacterViewUI
{
    public static AgentPromotionWindowUI Instance;

    [SerializeField]
    SelectableCharacterNodeTreeUI Tree;

    Character TopCharacter;

    Predicate<Character> CurrentFilter;

    protected override void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("PassTimeComplete", Refresh);
        CORE.Instance.SubscribeToEvent("CreditStolen", Refresh);
    }

    void Refresh()
    {
        if (!this.gameObject.activeInHierarchy)
        {
            return;
        }

        Show(null, CurrentFilter);
    }

    public override void Show(Action<Character> onSelect = null, Predicate<Character> filter = null, string title = "Select Agent:", Character topCharacter = null, AgentAction agentAction = null, AgentInteractable relevantTarget = null)
    {
        CurrentFilter = filter;
        this.gameObject.SetActive(true);
        base.Show(onSelect, CurrentFilter);
        TitleText.text = title;
    }

    protected override IEnumerator PopulateGrid(Action<Character> onSelect = null, Predicate<Character> filter = null)
    {

        yield return 0;

        if(TopCharacter == null)
        {
            TopCharacter = CORE.PC;
        }

        Tree.SetSelectableCharacters(TopCharacter, onSelect);

        PopulateGridRoutine = null;
    }
}
