using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectAgentWindowUI : SelectCharacterViewUI
{
    public static SelectAgentWindowUI Instance;

    [SerializeField]
    SelectableCharacterNodeTreeUI Tree;

    protected void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }
    
    public override void Show(Action<Character> onSelect, Predicate<Character> filter)
    {
        this.gameObject.SetActive(true);
        base.Show(onSelect, filter);
    }

    protected override IEnumerator PopulateGrid(Action<Character> onSelect = null, Predicate<Character> filter = null)
    {

        yield return 0;

        Tree.SetSelectableCharacters(CORE.PC, onSelect);

        PopulateGridRoutine = null;
    }
}
