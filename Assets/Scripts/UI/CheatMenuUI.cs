using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatMenuUI : MonoBehaviour
{
    public bool ShowAllCharactersOn = false;

    private void Start()
    {
        GameClock.Instance.OnTurnPassed.AddListener(ShowAllCharacters);
        CORE.Instance.SubscribeToEvent("PassTimeComplete", ShowAllCharacters);
    }

    public void ShowAllCharacters()
    {
        if(!ShowAllCharactersOn)
        {
            return;
        }

        foreach(Character character in CORE.Instance.Characters)
        {
            character.Known.KnowEverything(CORE.PC);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ShowAllCharactersOn = !ShowAllCharactersOn;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            CORE.PC.Gold += 100;
            CORE.PC.Connections += 100;
            CORE.PC.Rumors += 100;
        }
    }


}
