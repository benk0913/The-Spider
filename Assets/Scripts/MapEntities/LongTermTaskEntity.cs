using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongTermTaskEntity : AgentInteractable
{
    [SerializeField]
    public LongTermTask CurrentTask;

    [SerializeField]
    public Character CurrentCharacter;

    [SerializeField]
    public Character CurrentRequester;

    [SerializeField]
    public AgentInteractable CurrentTarget;

    [SerializeField]
    LongTermTaskDurationUI DurationEntity;

    public int TurnsLeft;

    public void SetInfo(LongTermTask task, Character requester, Character character, AgentInteractable target)
    {
        this.CurrentCharacter = character;
        this.CurrentRequester = requester;
        this.CurrentTask = task;
        this.CurrentTarget = target;
        TurnsLeft = task.TurnsToComplete;

        GameClock.Instance.OnTurnPassed.AddListener(TurnPassed);

        DurationEntity = ResourcesLoader.Instance.GetRecycledObject("LongTermTaskWorld").GetComponent<LongTermTaskDurationUI>();
        DurationEntity.transform.SetParent(CORE.Instance.MainCanvas.transform);
        DurationEntity.SetInfo(this);
    }

    private void OnDisable()
    {
        DurationEntity.gameObject.SetActive(false);
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

        DurationEntity.Refresh();
    }

    public void Complete()
    {
        AgentAction resultAction = CurrentTask.GetResult(CurrentCharacter);
        resultAction.Execute(CurrentRequester, CurrentCharacter, CurrentTarget);

        this.gameObject.SetActive(false);
    }

    public void Cancel()
    {
        this.gameObject.SetActive(false);
    }

}
