using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PointAndClickEntity : MonoBehaviour
{
    [SerializeField]
    UnityEvent OnClickEvent = new UnityEvent();

    [SerializeField]
    UnityEvent OnHover = new UnityEvent();

    [SerializeField]
    UnityEvent OnUnhover = new UnityEvent();

    public void OnClick()
    {
        OnClickEvent.Invoke();
    }

    public void Hover()
    {
        OnHover.Invoke();
    }

    public void Unhover()
    {
        OnUnhover.Invoke();
    }
}
