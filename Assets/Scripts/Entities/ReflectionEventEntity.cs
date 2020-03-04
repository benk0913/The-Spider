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

    private void OnEnable()
    {
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
