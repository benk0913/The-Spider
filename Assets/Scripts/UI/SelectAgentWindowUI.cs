using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectAgentWindowUI : SelectCharacterViewUI
{
    public static SelectAgentWindowUI Instance;

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

    public override bool CommonFilter(Character character)
    {
        return (character.TopEmployer == CORE.PC);
    }
}
