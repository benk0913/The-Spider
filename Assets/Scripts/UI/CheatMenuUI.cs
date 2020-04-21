using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatMenuUI : MonoBehaviour
{
    public bool ShowAllCharactersOn = false;

    public bool IsCheatOn = false;

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
        if (Input.GetKeyDown(KeyCode.End))
        {
            IsCheatOn = !IsCheatOn;
        }

        if(!IsCheatOn)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ShowAllCharactersOn = !ShowAllCharactersOn;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            CORE.PC.CGold += 100;
            CORE.PC.CConnections += 100;
            CORE.PC.CRumors += 100;
            CORE.PC.CProgress += 100;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            CORE.Instance.TechTree.FindAll().ForEach(x => x.IsResearched = true);
            CORE.Instance.InvokeEvent("ResearchComplete");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Quest quest = QuestsPanelUI.Instance.ActiveQuests.Find(x => x.Tutorial);
            foreach(QuestObjective objective in quest.Objectives)
            {
                if (objective.ValidateRoutine != null)
                {
                    StopCoroutine(objective.ValidateRoutine);
                    objective.ValidateRoutine = null;
                }

                objective.Complete();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) // Attackers Win Duel
        {
            PlottingDuelUI.Instance.TargetsPortraits.Clear();
            PlottingDuelUI.Instance.ExecuteDuelResult();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) // Defenders Win Dueel
        {
            PlottingDuelUI.Instance.ParticipantsPortraits.Clear();
            PlottingDuelUI.Instance.ExecuteDuelResult();
        }
        else if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            foreach(LocationEntity location in CORE.Instance.Locations)
            {
                location.PurchasePlot(CORE.PC, CORE.PC.PropertiesOwned[0].EmployeesCharacters[0]);
            }
        }
    }


}
