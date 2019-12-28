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

        foreach (LocationEntity location in CORE.Instance.Locations)
        {
            location.Known.KnowEverything(CORE.PC);
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
            CORE.PC.Progress += 100;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            CORE.Instance.TechTree.FindAll().ForEach(x => x.IsResearched = true);
            CORE.Instance.InvokeEvent("ResearchComplete");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Quest quest = QuestsPanelUI.Instance.ActiveQuests.Find(x => x.Tutorial);
            QuestsPanelUI.Instance.QuestComplete(quest);
        }
    }


}
