using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectAgentWindowUI : SelectCharacterViewUI
{
    public static SelectAgentWindowUI Instance;

    [SerializeField]
    SelectableCharacterNodeTreeUI Tree;

    Character TopCharacter;

    AgentAction RelevantAction = null;
    AgentInteractable RelevantTarget = null;

    protected override void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

   

    public override void Show(Action<Character> onSelect, Predicate<Character> filter, string title = "Select Agent:", Character topCharacter = null, AgentAction agentAction = null, AgentInteractable relevantTarget = null)
    {
        TopCharacter = topCharacter;
        RelevantAction = agentAction;
        RelevantTarget = relevantTarget;

        this.gameObject.SetActive(true);
        base.Show(onSelect, filter);
        TitleText.text = title;
    }

    protected override IEnumerator PopulateGrid(Action<Character> onSelect = null, Predicate<Character> filter = null)
    {

        yield return 0;

        if(TopCharacter == null)
        {
            TopCharacter = CORE.PC;
        }

        Tree.SetSelectableCharacters(TopCharacter, onSelect, RelevantAction, RelevantTarget);

        PopulateGridRoutine = null;
    }
}
