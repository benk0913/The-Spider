using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.EventSystems;

public class LongTermTaskEntity : AgentInteractable, IPointerClickHandler, ISaveFileCompatible
{
    public string ID;

    [SerializeField]
    public LongTermTask CurrentTask;

    [SerializeField]
    public Character CurrentCharacter;

    [SerializeField]
    public Character CurrentRequester;

    [SerializeField]
    public AgentInteractable CurrentTarget;

    public int TurnsLeft;

    LocationEntity CurrentLocation;

    void Awake()
    {
        ID = Util.GenerateUniqueID();
    }

    private void Start()
    {
        CORE.Instance.LongTermTasks.Add(this);
    }


    public void SetInfo(LongTermTask task, Character requester, Character character, AgentInteractable target)
    {
        this.CurrentCharacter = character;
        this.CurrentRequester = requester;
        this.CurrentTask = task;
        this.CurrentTarget = target;
        TurnsLeft = task.TurnsToComplete;

        this.CurrentCharacter.StartDoingTask(this);

        GameClock.Instance.OnTurnPassed.AddListener(TurnPassed);

        CurrentLocation = ((LocationEntity)target);

        CurrentLocation.AddLongTermTask(this);
    }

    private void OnDisable()
    {
        GameClock.Instance.OnTurnPassed.RemoveListener(TurnPassed);
        CORE.Instance.LongTermTasks.Remove(this);
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
        resultAction.Execute(CORE.Instance.Database.GOD, CurrentCharacter, CurrentTarget);

        this.gameObject.SetActive(false);
    }

    public bool Cancel()
    {
        if(!CurrentTask.Cancelable)
        {
            return false;
        }

        CurrentLocation.RemoveLongTermTask(this);
        CurrentCharacter.StopDoingCurrentTask();
        this.gameObject.SetActive(false);

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

    string _currentCharacterID;
    string _currentRequesterID;
    string _currentTargetLocationID;
    string _currentLocationID;

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        node["ID"] = ID;
        node["CurrentTask"] = CurrentTask.name;
        node["CurrentCharacter"] = CurrentCharacter.ID;
        node["CurrentRequester"] = CurrentRequester.ID;

        if (CurrentTarget.GetType() == typeof(LocationEntity))
        {
            node["CurrentTargetLocation"] = ((LocationEntity)CurrentTarget).ID;
        }
        else
        {
            Debug.LogError("SUPPORT FOR NON LOCATION LONG-TERM-TASKS NOT IMPLEMENTED!");
        }

        node["TurnsLeft"] = TurnsLeft.ToString();

        node["CurrentLocation"] = CurrentLocation.ID;


        return node;
    }

    public void FromJSON(JSONNode node)
    {
        ID = node["ID"];
        CurrentTask = CORE.Instance.Database.GetTask(node["CurrentTask"]);


    }

    public void ImplementIDs()
    {
        throw new System.NotImplementedException();
    }
}
