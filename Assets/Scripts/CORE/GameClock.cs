using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClock : MonoBehaviour
{
    public static GameClock Instance;

    public GameTime CurrentTime = GameTime.Morning;

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
        }

        CORE.Instance.InvokeEvent(CurrentTime.ToString());
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
}
