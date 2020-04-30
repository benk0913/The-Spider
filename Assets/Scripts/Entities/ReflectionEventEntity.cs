using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ReflectionEventEntity : MonoBehaviour
{
    public UnityEvent OnEnableEvent;
    public UnityEvent OnAwakeEvent;
    public UnityEvent OnStartEvent;
    public UnityEvent OnDisableEvent;
    public bool isRequireCORE = false;

    private void OnEnable()
    {
        if(isRequireCORE && CORE.Instance == null)
        {
            return;
        }

        OnEnableEvent.Invoke();
    }

    private void Awake()
    {
        OnAwakeEvent.Invoke();
    }
    
    void Start()
    {
        OnStartEvent.Invoke();
    }

    private void OnDisable()
    {
        OnDisableEvent.Invoke();
    }
}
