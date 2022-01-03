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

    public bool IsFocusing = false;

    public void Activate()
    {
        if(CORE.Instance.FocusViewLocked)
        {
            return;
        }

        if(MouseLook.Instance.CurrentWindow != null)
        {
            return;
        }

        this.gameObject.SetActive(true);
        MouseLook.Instance.FocusOnView(this);
        OnActivate.Invoke();

        IsFocusing = true;
    }
    
    public void Deactivate()
    {

        if (CORE.Instance.FocusViewLocked)
        {
            if(CORE.Instance.DEBUG) Debug.LogError("Can't unfocus "+CORE.Instance.FocusViewLockers[0].name);
            return;
        }

        if(MouseLook.Instance.CurrentWindow != null)
        {
            if(CORE.Instance.DEBUG) Debug.LogError("Can't unfocus | Current Window: "+MouseLook.Instance.CurrentWindow.gameObject.name);
            return;
        }

        MouseLook.Instance.UnfocusCurrentView();
        this.gameObject.SetActive(false);
        OnDeactivate.Invoke();

        IsFocusing = false;
    }


}
