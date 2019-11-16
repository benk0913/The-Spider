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

    public void SetInfo(LongTermTask task, Character requester, Character character, LocationEntity targetLocation, Character targetCharacter = null, int turnsLeft = -1)
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
    }

    public void RefreshKnownTaskState()
    {
        isKnownTask = (CurrentCharacter.TopEmployer == CORE.PC
            || ((CurrentCharacter.isImportant || CurrentCharacter.CurrentFaction != CORE.Instance.Database.DefaultFaction) && CurrentCharacter.IsKnown("CurrentLocation", CORE.PC)));
    }

    public void TurnPassed()
    {
        TurnsLeft--;

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
