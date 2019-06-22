using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FocusView : MonoBehaviour
{
    [SerializeField]
    public Camera CurrentCamera;

    [SerializeField]
    UnityEvent OnActivate = new UnityEvent();
    [SerializeField]
    UnityEvent OnDeactivate = new UnityEvent();

    public void Activate()
    {
        this.gameObject.SetActive(true);
        MouseLook.Instance.FocusOnView(this);
        OnActivate.Invoke();
    }
    
    public void Deactivate()
    {
        MouseLook.Instance.UnfocusCurrentView();
        this.gameObject.SetActive(false);
        OnDeactivate.Invoke();
    }

}
