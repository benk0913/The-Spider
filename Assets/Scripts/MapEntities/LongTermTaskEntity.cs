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

        this.CurrentCharacter.StartDoingTask(this);

        GameClock.Instance.OnTurnPassed.AddListener(TurnPassed);
        
        CurrentLocation = ((LocationEntity)targetLocation);

        CurrentLocation.AddLongTermTask(this);
    }

    private void OnDestroy()
    {
        GameClock.Instance.OnTurnPassed.RemoveListener(TurnPassed);
    }

    void TurnPassed()
    {
        TurnsLeft--;

        if (TurnsLeft <= 0)
        {
            Complete();
            return;
        }

        if(CurrentLocation.TaskDurationUI == null)
        {
            return;
        }

        CurrentLocation.TaskDurationUI.Refresh();
    }

    public void Complete()
    {
        CurrentLocation.RemoveLongTermTask(this);

        this.CurrentCharacter.StopDoingCurrentTask(true);

        AgentAction resultAction = CurrentTask.GetResult(CurrentCharacter);

        if(TargetCharacter != null)
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
        

        Destroy(this.gameObject);
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
