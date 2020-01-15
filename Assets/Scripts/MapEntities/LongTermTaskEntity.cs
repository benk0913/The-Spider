using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.EventSystems;

public class LongTermTaskEntity : AgentInteractable, IPointerClickHandler
{
    [SerializeField]
    public LongTermTask CurrentTask;

    [SerializeField]
    public Character CurrentCharacter;

    [SerializeField]
    public Character CurrentRequester;

    [SerializeField]
    public Character TargetCharacter;

    [SerializeField]
    public LocationEntity CurrentTargetLocation;


    public int TurnsLeft;

    LocationEntity CurrentLocation;

    public bool isKnownTask;

    public bool isComplete;

    AgentAction ActionPerTurn;

    public bool HaltWhenActionPerTurnExecuted;

    public void SetInfo(LongTermTask task, Character requester, Character character, LocationEntity targetLocation, Character targetCharacter = null, int turnsLeft = -1, AgentAction actionPerTurn = null)
    {
        if(turnsLeft > 0)
        {
            this.TurnsLeft = turnsLeft;
        }

        this.TargetCharacter = targetCharacter;
        this.CurrentCharacter = character;
        this.CurrentRequester = requester;
        this.CurrentTask = task;
        this.CurrentTargetLocation = targetLocation;
        TurnsLeft = task.TurnsToComplete;

        RefreshKnownTaskState();

        this.CurrentCharacter.StartDoingTask(this);
            

        CurrentLocation = ((LocationEntity)targetLocation);

        CurrentLocation.AddLongTermTask(this);

        this.ActionPerTurn = actionPerTurn;
        HaltWhenActionPerTurnExecuted = CurrentTask.HaltWhenActionPerTurnExecuted;

        if(CurrentTask.ShowCharacterInWorld)
        {
            LocationEntity currLocation = character.CurrentLocation;
            character.GoToLocation(CORE.Instance.Locations[0]);
            character.GoToLocation(currLocation);
        }
    }

    public void RefreshKnownTaskState()
    {
        isKnownTask = CurrentCharacter.TopEmployer == CORE.PC
            || CurrentCharacter.isImportant
            || (CurrentCharacter.CurrentFaction != CORE.Instance.Factions.Find(x=>x.name == CORE.Instance.Database.DefaultFaction.name) && CurrentCharacter.IsKnown("CurrentLocation", CORE.PC));
    }

    public void TurnPassed()
    {
        TurnsLeft--;

        if(ActionPerTurn != null)
        {
            PortraitUI characterContainer = ResourcesLoader.Instance.GetRecycledObject("PortraitUI").GetComponent<PortraitUI>();
            characterContainer.SetCharacter(TargetCharacter);

            FailReason reason = null;
            if (ActionPerTurn.CanDoAction(CurrentRequester, CurrentCharacter, characterContainer, out reason))
            {
                ActionPerTurn.Execute(CurrentRequester, CurrentCharacter, characterContainer);

                if (HaltWhenActionPerTurnExecuted)
                {
                    Dispose();
                }
            }
        }

        if (TurnsLeft <= 0 && !isComplete)
        {
            Complete();
            return;
        }
    }

    public void Complete()
    {
        this.CurrentCharacter.StopDoingCurrentTask(true);

        AgentAction resultAction = CurrentTask.GetResult(CurrentCharacter);

        if (TargetCharacter != null)
        {
            PortraitUI tempPortrait = ResourcesLoader.Instance.GetRecycledObject("PortraitUI").GetComponent<PortraitUI>();
            tempPortrait.SetCharacter(TargetCharacter);
            tempPortrait.transform.position = Vector3.zero;

            resultAction.Execute(CORE.Instance.Database.GOD, CurrentCharacter, tempPortrait);

            tempPortrait.gameObject.SetActive(false);
        }
        else
        {
            resultAction.Execute(CORE.Instance.Database.GOD, CurrentCharacter, CurrentTargetLocation);
        }

    }

    public void Dispose()
    {
        CurrentLocation.RemoveLongTermTask(this);
       
        if (!isComplete)
        {
            Destroy(this.gameObject);
        }

        isComplete = true;
    }

    public bool Cancel()
    {
        if(!CurrentTask.Cancelable)
        {
            return false;
        }

        CurrentLocation.RemoveLongTermTask(this);

        CurrentCharacter.StopDoingCurrentTask();

        Destroy(this.gameObject);

        return true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            ShowActionMenu();
        }
    }

    public void ShowActionMenu()
    {

    }

}
