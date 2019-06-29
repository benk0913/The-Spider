using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PointAndClickEntity : MonoBehaviour
{
    [SerializeField]
    protected UnityEvent OnClickEvent = new UnityEvent();

    [SerializeField]
    protected UnityEvent OnHover = new UnityEvent();

    [SerializeField]
    protected UnityEvent OnUnhover = new UnityEvent();

    public void OnClick()
    {
        OnClickEvent.Invoke();
    }

    public void Hover()
    {
        OnHover.Invoke();
        InternalHover();
    }

    public void Unhover()
    {
        OnUnhover.Invoke();
        InternalUnhover();
    }

    public virtual void InternalHover()
    {

    }

    public virtual void InternalUnhover()
    {

    }
}
