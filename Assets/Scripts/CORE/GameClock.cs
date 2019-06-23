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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("PassTime", PassTime);
    }

    public void PassTime()
    {
        CurrentTime++;

        if(CurrentTime > GameTime.Night)
        {
            CurrentTime = GameTime.Morning;
            DaysPast++;
        }

        CORE.Instance.InvokeEvent(CurrentTime.ToString());
        OnTurnPassed.Invoke();
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
            Days = Mathf.FloorToInt((float)turns / 5f);
            DayTime = turns % 5;
        }
    }
}
