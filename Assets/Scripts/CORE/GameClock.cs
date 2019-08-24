using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameClock : MonoBehaviour
{
    public static GameClock Instance;

    public GameTime CurrentTime = GameTime.Morning;

    public int DaysPast = 0;

    public UnityEvent OnTurnPassed = new UnityEvent();

    public UnityEvent OnDayPassed = new UnityEvent();

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
        CurrentTime++;

        if(CurrentTime > GameTime.Night)
        {
            CurrentTime = GameTime.Morning;
            DaysPast++;

            OnDayPassed.Invoke();
        }

        CORE.Instance.InvokeEvent(CurrentTime.ToString());
        OnTurnPassed.Invoke();

        GlobalMessagePrompterUI.Instance.Show(CurrentTime.ToString(), 1f);
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
            Days = Mathf.FloorToInt((float)(turns + GameClock.Instance.CurrentTime) / 5f);

            DayTime = (int)GameClock.Instance.CurrentTime + (turns - (Days * 5));
        }
    }
}
