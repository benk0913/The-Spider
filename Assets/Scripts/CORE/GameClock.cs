using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;

public class GameClock : MonoBehaviour, ISaveFileCompatible
{
    public static GameClock Instance;

    public GameTime CurrentTimeOfDay = GameTime.Morning;

    public int CurrentWeek
    {
        get
        {
            return Mathf.FloorToInt(CurrentDay / 7f);
        }
    }

    public int CurrentDay
    {
        get
        {
            return Mathf.FloorToInt(CurrentTurn / 5f);
        }
    }

    public int CurrentTurn = 0;

    public UnityEvent OnTurnPassed = new UnityEvent();

    public UnityEvent OnDayPassed = new UnityEvent();

    public UnityEvent OnWeekPassed = new UnityEvent();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        CORE.Instance.SubscribeToEvent("PassTime", PassTime);
    }

    public void Wipe()
    {
        OnTurnPassed.RemoveAllListeners();
        OnDayPassed.RemoveAllListeners();
    }

    public void PassTime()
    {
        if (PassTimeRoutineInstance != null)
        {
            return;
        }

        PassTimeRoutineInstance = StartCoroutine(PassTimeRoutine());
    }

    public Coroutine PassTimeRoutineInstance;
    IEnumerator PassTimeRoutine()
    {
        AudioControl.Instance.Play("turn_pass");
        AudioControl.Instance.Play("turn_pass_loading");
        CORE.Instance.InvokeEvent("PassTimeStarted");

        int currentWeek = CurrentWeek;

        CurrentTimeOfDay++;
        CurrentTurn++;

        if (CurrentTimeOfDay > GameTime.Night)
        {
            CurrentTimeOfDay = GameTime.Morning;

            OnDayPassed.Invoke();
        }

        if (currentWeek != CurrentWeek)
        {
            OnWeekPassed.Invoke();
        }

        CORE.Instance.InvokeEvent(CurrentTimeOfDay.ToString());
        OnTurnPassed.Invoke();

        GlobalMessagePrompterUI.Instance.Show(CurrentTimeOfDay.ToString(), 1f);

        yield return 0;

        while (CORE.Instance.TurnPassedRoutineInstance != null)
        {
            yield return 0;
        }

        CORE.Instance.InvokeEvent("PassTimeComplete");

        AudioControl.Instance.StopSound("turn_pass_loading");
        PassTimeRoutineInstance = null;
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        node["CurrentTurn"] = CurrentTurn.ToString();

        return node;
    }

    public void FromJSON(JSONNode node)
    {
        CurrentTurn = int.Parse(node["CurrentTurn"]);
    }

    public void ImplementIDs()
    {
    }

    [System.Serializable]
    public enum GameTime
    {
        Morning,
        Noon,
        Afternoon,
        Evening,
        Night
    }

    public class GameTimeLength
    {
        public int Days;
        public int DayTime;

        public GameTimeLength(int turns)
        {
            //Days = Mathf.FloorToInt((float)turns / 5f);
            Days = Mathf.FloorToInt((float)(turns + GameClock.Instance.CurrentTimeOfDay) / 5f);

            DayTime = (int)GameClock.Instance.CurrentTimeOfDay + (turns - (Days * 5));
        }
    }



}
