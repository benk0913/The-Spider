using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameStorage : MonoBehaviour
{
    public static GameStorage Instance;

    public int Gold
    {
        get
        {
            return _gold;
        }
        set
        {
            _gold = value;
            OnMoneyChanged.Invoke();
        }
    }
    int _gold;

    public int Reputation
    {
        get
        {
            return _rep;
        }
        set
        {
            _rep = value;
            OnReputationChanged.Invoke();
        }
    }
    int _rep;

    public UnityEvent OnMoneyChanged = new UnityEvent();
    public UnityEvent OnReputationChanged = new UnityEvent();

    private void Awake()
    {
        Instance = this;
    }

}
